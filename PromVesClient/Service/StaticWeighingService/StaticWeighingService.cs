using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PromVesClient.Service.StaticWeighingService
{
    public class StaticWeighingService
    {
        private readonly ILogger<StaticWeighingService> _logger;

        private readonly ApplicationDbContext _dbContext;

        private const decimal DefaultLoadCapacity = 70;

        //private Guid IdReceipt;
        //private decimal Platform1Left { get; set; }
        //private decimal Platform1Right { get; set; }
        //private decimal Platform2Left { get; set; }
        //private decimal Platform2Right { get; set; }
        public StaticWeighingService(ILogger<StaticWeighingService> logger, ApplicationDbContext dbContext)
        { 
            _logger = logger;
            _dbContext = dbContext;
        }
        //метод отвечающий за сохранение данных взвешивания WeighingDto dto
        //public async Task<ServiceResult> saveWeighingAsync(decimal Platform1Left, decimal Platform1Right, decimal Platform2Left, decimal Platform2Right, string VagonNumber, decimal TareWeight, decimal GrossWeight)
        public async Task<ServiceResult> saveWeighingAsync(WeighingDto dtoWeighing)
        {
            //Запись сторон платформы
            decimal L1 = dtoWeighing.Platform1Left;
            decimal R1 = dtoWeighing.Platform1Right;
            decimal L2 = dtoWeighing.Platform2Left;
            decimal R2 = dtoWeighing.Platform2Right;
            //общая сумма в весов
            decimal WeightSum = dtoWeighing.Platform1Left + dtoWeighing.Platform1Right + dtoWeighing.Platform2Left + dtoWeighing.Platform2Right;
            //грузопольемность
            //decimal LoadCapacity = 70;
            //расчет переруза/недогруза
            decimal LoadDeviation = DefaultLoadCapacity - WeightSum;
            //временно Нетто 0
            decimal NetWeight = 0;
            //первая тележка
            decimal FirstCart = dtoWeighing.Platform1Left + dtoWeighing.Platform1Right;
            //вторая тележка
            decimal SecondCart = dtoWeighing.Platform2Left + dtoWeighing.Platform2Right;
            //разница тележек
            decimal DifferenceCarts = FirstCart - SecondCart;
            //вес левого борта
            decimal LeftSide = dtoWeighing.Platform1Left + dtoWeighing.Platform2Left;
            //вес правого борта
            decimal RightSide = dtoWeighing.Platform1Right + dtoWeighing.Platform2Right;
            //разница бортов
            decimal DifferenceSides = Math.Abs(LeftSide - RightSide);
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
                    _logger.LogInformation(
                    "Расчет нетто: Gross={Gross}, Tare={Tare}, Result={Result}",
                    dtoWeighing.GrossWeight,
                    lastWeighing.TareWeight,
                    dtoWeighing.GrossWeight - lastWeighing.TareWeight);
                    //вычесление нетто, если есть Тара и сохраняем Брутто в текущую запись
                    if (dtoWeighing.TareWeight != 0 && lastWeighing.GrossWeight != 0)
                    {
                        NetWeight =  Math.Round(lastWeighing.GrossWeight - dtoWeighing.TareWeight, 2,
                        MidpointRounding.AwayFromZero); 
                        dtoWeighing.GrossWeight = lastWeighing.GrossWeight;
                    }
                    //вычесление нетто, если есть Брутто и сохраняем Тару в текущую запись
                    else if (dtoWeighing.GrossWeight != 0 && lastWeighing.TareWeight != 0)
                    {
                        NetWeight = Math.Round(dtoWeighing.GrossWeight - lastWeighing.TareWeight,
                        2,
                        MidpointRounding.AwayFromZero);
                        //NetWeight = Math.Truncate((dtoWeighing.GrossWeight - lastWeighing.TareWeight) * 100) / 100.0; ;
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
                L1 = L1,
                R1 = R1,
                L2 = L2,
                R2 = R2,
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
                TypeWeighing = dtoWeighing.TypeWeighing,
                Shipper = dtoWeighing.Shipper,
                Сonsignee = dtoWeighing.Сonsignee,
                Сargo = dtoWeighing.Сargo,
                InvoiceNumber = dtoWeighing.InvoiceNumber,
                InvoiceDataTime = dtoWeighing.InvoiceDataTime?.ToUniversalTime(),
                InvoiceWeighing = dtoWeighing.InvoiceWeighing,
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
        public async Task<List<Image>> GetImageWeighingAsync(decimal weightSum)
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
        //получение локального ip адреса компьютера
        public async Task<IPAddress> GetLocalIPAddressAsync()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }

            throw new Exception("Локальный IPv4 адрес не найден.");
        }

       
    }
}
