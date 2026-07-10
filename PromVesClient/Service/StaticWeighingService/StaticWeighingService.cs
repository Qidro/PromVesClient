using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace PromVesClient.Service.StaticWeighingService
{
    public class StaticWeighingService
    {
        private readonly ILogger<StaticWeighingService> _logger;

        private readonly ApplicationDbContext _dbContext;

        private const double DefaultLoadCapacity = 70;
        //private double Platform1Left { get; set; }
        //private double Platform1Right { get; set; }
        //private double Platform2Left { get; set; }
        //private double Platform2Right { get; set; }
        public StaticWeighingService(ILogger<StaticWeighingService> logger, ApplicationDbContext dbContext)
        { 
            _logger = logger;
            _dbContext = dbContext;
        }
        //метод отвечающий за сохранение данных взвешивания
        public async Task<ServiceResult> saveWeighingAsync(Guid Id,double Platform1Left, double Platform1Right, double Platform2Left, double Platform2Right, string VagonNumber, double TareWeight, double GrossWeight)
        {
            //общая сумма в весов
            double WeightSum = Platform1Left + Platform1Right + Platform2Left + Platform2Right;
            //грузопольемность
            //double LoadCapacity = 70;
            //расчет переруза/недогруза
            double LoadDeviation = DefaultLoadCapacity - WeightSum;
            //временно Нетто 0
            double NetWeight = 0;
            //первая тележка
            double FirstCart = Platform1Left + Platform1Right;
            //вторая тележка
            double SecondCart = Platform2Left + Platform2Right;
            //разница тележек
            double DifferenceCarts = FirstCart - SecondCart;
            //вес левого борта
            double LeftSide = Platform1Left + Platform2Left;
            //вес правого борта
            double RightSide = Platform1Right + Platform2Right;
            //разница бортов
            double DifferenceSides = Math.Abs(LeftSide - RightSide);
            try
            {
                //поиск последней записи по номеру вагона
                var lastWeighing = await _dbContext.Weighings
            .Include(w => w.Receipt)
            .Where(w => w.VagonNumber == VagonNumber)
            .OrderByDescending(w => w.Receipt.DateTime)
            .FirstOrDefaultAsync();

                //если запись есть вычисляем нетто
                if (lastWeighing != null)
                {
                    //вычесление нетто, если есть Тара и сохраняем Брутто в текущую запись
                    if (TareWeight != 0 && lastWeighing.GrossWeight != 0)
                    {
                        NetWeight = lastWeighing.GrossWeight - TareWeight;
                        GrossWeight = lastWeighing.GrossWeight;
                    }
                    //вычесление нетто, если есть Брутто и сохраняем Тару в текущую запись
                    else if (GrossWeight != 0 && lastWeighing.TareWeight != 0)
                    {
                        NetWeight = GrossWeight - lastWeighing.TareWeight;
                        TareWeight = lastWeighing.TareWeight;
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
                VagonNumber = VagonNumber,
                TareWeight = TareWeight,
                GrossWeight = GrossWeight,
                NetWeight = NetWeight,
                LoadCapacity = DefaultLoadCapacity,
                LoadDeviation = LoadDeviation,
                FirstCart = FirstCart,
                SecondCart = SecondCart,
                DifferenceCarts = DifferenceCarts,
                LeftSide = LeftSide,
                RightSide = RightSide,
                DifferenceSides = DifferenceSides,
                ReceiptId = Id
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
                Id = Guid.NewGuid(),
                DateTime = DateTime.Now,
                TypeWeighng = TypeWeighing,
                Operator = Operator
            };

            try
            {
                _dbContext.Receipts.Add(receipt);
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
    }
}
