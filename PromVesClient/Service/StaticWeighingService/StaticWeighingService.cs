using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PromVesClient.Service.StaticWeighingService
{
    public class StaticWeighingService
    {
        private readonly ILogger<StaticWeighingService> _logger;

        private readonly ApplicationDbContext _dbContext;

        private const double DefaultLoadCapacity = 70;

        //private Guid IdReceipt;
        //private double Platform1Left { get; set; }
        //private double Platform1Right { get; set; }
        //private double Platform2Left { get; set; }
        //private double Platform2Right { get; set; }
        public StaticWeighingService(ILogger<StaticWeighingService> logger, ApplicationDbContext dbContext)
        { 
            _logger = logger;
            _dbContext = dbContext;
        }
        //метод отвечающий за сохранение данных взвешивания WeighingDto dto
        //public async Task<ServiceResult> saveWeighingAsync(double Platform1Left, double Platform1Right, double Platform2Left, double Platform2Right, string VagonNumber, double TareWeight, double GrossWeight)
        public async Task<ServiceResult> saveWeighingAsync(WeighingDto dtoWeighing)
        {
            //общая сумма в весов
            double WeightSum = dtoWeighing.Platform1Left + dtoWeighing.Platform1Right + dtoWeighing.Platform2Left + dtoWeighing.Platform2Right;
            //грузопольемность
            //double LoadCapacity = 70;
            //расчет переруза/недогруза
            double LoadDeviation = DefaultLoadCapacity - WeightSum;
            //временно Нетто 0
            double NetWeight = 0;
            //первая тележка
            double FirstCart = dtoWeighing.Platform1Left + dtoWeighing.Platform1Right;
            //вторая тележка
            double SecondCart = dtoWeighing.Platform2Left + dtoWeighing.Platform2Right;
            //разница тележек
            double DifferenceCarts = FirstCart - SecondCart;
            //вес левого борта
            double LeftSide = dtoWeighing.Platform1Left + dtoWeighing.Platform2Left;
            //вес правого борта
            double RightSide = dtoWeighing.Platform1Right + dtoWeighing.Platform2Right;
            //разница бортов
            double DifferenceSides = Math.Abs(LeftSide - RightSide);
            try
            {
                //поиск последней записи по номеру вагона
                var lastWeighing = await _dbContext.Weighings
            .Include(w => w.Receipt)
            .Where(w => w.VagonNumber == dtoWeighing.VagonNumber)
            .OrderByDescending(w => w.Receipt.DateTime)
            .FirstOrDefaultAsync();

                //если запись есть вычисляем нетто
                if (lastWeighing != null)
                {
                    //вычесление нетто, если есть Тара и сохраняем Брутто в текущую запись
                    if (dtoWeighing.TareWeight != 0 && lastWeighing.GrossWeight != 0)
                    {
                        NetWeight = lastWeighing.GrossWeight - dtoWeighing.TareWeight;
                        dtoWeighing.GrossWeight = lastWeighing.GrossWeight;
                    }
                    //вычесление нетто, если есть Брутто и сохраняем Тару в текущую запись
                    else if (dtoWeighing.GrossWeight != 0 && lastWeighing.TareWeight != 0)
                    {
                        NetWeight = dtoWeighing.GrossWeight - lastWeighing.TareWeight;
                        dtoWeighing.TareWeight = lastWeighing.TareWeight;
                    }
                }
            } 
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Ошибка выполнения запроса: " + ex.Message);
                return ServiceResult.Fail("Ошибка выполнения запроса: " + ex.Message);
                //  return ServiceResult.Fail("Ошибка выполнения запроса.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка получения данных: " + ex.Message);
                return ServiceResult.Fail("Ошибка получения данных: "+ ex.Message);
            }
            

            //записываем в модель данные взвешивания
            var weighingResult = new Weighing
            { 
                Id = Guid.NewGuid(),
                VagonNumber = dtoWeighing.VagonNumber,
                TareWeight = dtoWeighing.TareWeight,
                GrossWeight = dtoWeighing.GrossWeight,
                NetWeight = NetWeight,
                LoadCapacity = DefaultLoadCapacity,
                LoadDeviation = LoadDeviation,
                FirstCart = FirstCart,
                SecondCart = SecondCart,
                DifferenceCarts = DifferenceCarts,
                LeftSide = LeftSide,
                RightSide = RightSide,
                DifferenceSides = DifferenceSides,
                ReceiptId = dtoWeighing.IdReceipt
            };
            //сохраняем данные
            try
            {
                _dbContext.Weighings.Add(weighingResult);
                await _dbContext.SaveChangesAsync();
                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Ошибка обновления БД:" + ex.Message);
                return ServiceResult.Fail("Ошибка обновления БД: " + ex.Message);
            }

            catch (Exception ex)
            {
                return ServiceResult.Fail("Ошибка в записи в БД: " + ex.Message);
            }


        }
        //метод создания квитанции
        public async Task<ServiceResult> saveReceiptAsync(Guid Id, string TypeWeighing, string Operator)
        {
            var receipt = new Receipt
            {
                Id = Id,
                DateTime = DateTime.UtcNow,
                TypeWeighng = TypeWeighing,
                Operator = Operator
            };

            try
            {
                bool exists = await _dbContext.Receipts.AnyAsync(r => r.Id == receipt.Id);
                if (!exists)
                {
                    _dbContext.Receipts.Add(receipt);
                    await _dbContext.SaveChangesAsync();
                   // IdReceipt = Id;
                }
                else
                {
                    // Квитанция уже существует
                }
                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Ошибка обновления БД:" + ex.Message);
                return ServiceResult.Fail("Ошибка обновления БД: " + ex.Message);
            }

            catch (Exception ex)
            {
                return ServiceResult.Fail("Ошибка в записи в БД: " + ex.Message);
            }
            
        }
        //метод предназначен для получения коллекции изображений для табла общего веса
        public async Task<List<Image>> GetImageWeighingAsync(double weightSum)
        {
            List<Image> images = new List<Image>();
            //преобразуем массив в string формат
            string weightSumString = weightSum.ToString("F2");
            Console.WriteLine(weightSumString);
            //начиаем проход массива с конца
            for (int i = weightSumString.Length - 1; i >= 0; i--)
            {
                //вычисляем проход по цикла
                int iteration = weightSumString.Length - 1 - i;
                
                if (weightSumString.Length-1 < i)
                {
                    images.Add(Properties.Resources._00);
                }
                //проверяем на третьем проходе массива ли мы 
                if (iteration == 3)
                {
                    //сохраняем значения согласну элементу (значение с запятой)
                    switch (weightSumString[i])
                    {
                        case '0':
                            images.Add(Properties.Resources._0t);
                            break;
                        case '1':
                            images.Add(Properties.Resources._1t);
                            break;
                        case '2':
                            images.Add(Properties.Resources._2t);
                            break;
                        case '3':
                            images.Add(Properties.Resources._3t);
                            break;
                        case '4':
                            images.Add(Properties.Resources._4t);
                            break;
                        case '5':
                            images.Add(Properties.Resources._5t);
                            break;
                        case '6':
                            images.Add(Properties.Resources._6t);
                            break;
                        case '7':
                            images.Add(Properties.Resources._7t);
                            break;
                        case '8':
                            images.Add(Properties.Resources._8t);
                            break;
                        case '9':
                            images.Add(Properties.Resources._9t);
                            break;

                    }
                }
                else
                {
                    //сохраняем значения согласну элементу (значение без запятой)
                    switch (weightSumString[i])
                    {
                        case '0':
                            images.Add(Properties.Resources._0);
                            break;
                        case '1':
                            images.Add(Properties.Resources._1);
                            break;
                        case '2':
                            images.Add(Properties.Resources._2);
                            break;
                        case '3':
                            images.Add(Properties.Resources._3);
                            break;
                        case '4':
                            images.Add(Properties.Resources._4);
                            break;
                        case '5':
                            images.Add(Properties.Resources._5);
                            break;
                        case '6':
                            images.Add(Properties.Resources._6);
                            break;
                        case '7':
                            images.Add(Properties.Resources._7);
                            break;
                        case '8':
                            images.Add(Properties.Resources._8);
                            break;
                        case '9':
                            images.Add(Properties.Resources._9);
                            break;

                    }
                }
                //char u = weightSumString[i];
                if (i == 0 && iteration != 6)
                {
                    for (int j = 0; j < 6 - iteration; j++)
                    {
                        images.Add(Properties.Resources._00);
                    }
                }
            }
            
            return images;
        }
    }
}
