using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;
namespace PromVesClient.Service.ReceiptsService
{
    public class ReceiptsService
    {
        private readonly ILogger<ReceiptsService> _logger;
        private readonly ApplicationDbContext _dbContext;
        public ReceiptsService(ILogger<ReceiptsService> logger, ApplicationDbContext dbContext) 
        { 
            _logger = logger;
            _dbContext = dbContext;
        }
        //метод, который возвращает квитанции из БД, используется DTO квитанций - сокращенный набор данных
        public async Task<ServiceResult<List<ReceiptDto>>> GetReceiptsAsync()
        {
            var receipts = await _dbContext.Receipts
           .Select(r => new ReceiptDto
           {
               Id = r.Id,
               //переводим время Utc (посгрес сохраняем формат времени только в нем) в локальное время (наш часовой период)
               DateTime = DateTime.SpecifyKind(r.DateTime, DateTimeKind.Utc)
                                 .ToLocalTime(),

               TypeWeighng = r.TypeWeighng,
               Operator = r.Operator
           })
           .ToListAsync();
            //отправляем данные
            return new ServiceResult<List<ReceiptDto>>
            {
                Success = true,
                Data = receipts
            };
        }
        //получение всех карточек вагона
        public async Task<ServiceResult<List<Weighing>>> GetWeighingAsync()
        {
            var receipts = await _dbContext.Weighings.ToListAsync();
            return new ServiceResult<List<Weighing>>
            {
                Success = true,
                Data = receipts
            };
        }

        //поиск квитанций с помощью фильтра
        public async Task<ServiceResult<List<ReceiptDto>>> GetReceiptsFiltуrAsync(ReceiptDto _receiptDto)
        {
            var receipts = await _dbContext.Receipts
           .Select(r => new ReceiptDto
           {
               Id = r.Id,
               //переводим время Utc (посгрес сохраняем формат времени только в нем) в локальное время (наш часовой период)
               DateTime = DateTime.SpecifyKind(r.DateTime, DateTimeKind.Utc)
                                 .ToLocalTime(),

               TypeWeighng = r.TypeWeighng,
               Operator = r.Operator
           })
           .ToListAsync();
            //отправляем данные
            return new ServiceResult<List<ReceiptDto>>
            {
                Success = true,
                Data = receipts
            };
        }
        //метод предназначен для поиска взвешиваний с квитанции
        public async Task<ServiceResult<List<CardsDto>>> GetCardsAsync(Guid IdReceipt)
        {
            //заполняем данные
            var weighing = await _dbContext.Weighings
                .Where(w => w.ReceiptId == IdReceipt)
                .Select(r => new CardsDto
                {
                    Id = r.Id,
                    VagonNumber = r.VagonNumber,
                    TareWeight = r.TareWeight,
                    GrossWeight = r.GrossWeight,
                    NetWeight = r.NetWeight,
                    LoadCapacity = r.LoadCapacity,
                    LoadDeviation = r.LoadDeviation,
                    FirstCart = r.FirstCart,
                    SecondCart = r.SecondCart,
                    DifferenceCarts = r.DifferenceCarts,
                    LeftSide = r.LeftSide,
                    RightSide = r.RightSide,
                    DifferenceSides = r.DifferenceSides,
                    TypeWeighing = r.TypeWeighing,
                    ReceiptId = IdReceipt

                }).ToListAsync();

            return new ServiceResult<List<CardsDto>>
            {
                Success = true,
                Data = weighing
            };
        }

        public async Task<ServiceResult<List<ReceiptDto>>> GetReceiptFilter(SearchReceiptDto filter)
        {
            var query = _dbContext.Receipts.AsQueryable();

            // Период
            query = query.Where(r =>
                r.DateTime >= filter.periodStart &&
                r.DateTime <= filter.periodEnd);

            // Оператор
            if (!string.IsNullOrWhiteSpace(filter.operatorName))
            {
                query = query.Where(o => o.Operator == filter.operatorName);
            }

            // Номер вагона
            if (!string.IsNullOrWhiteSpace(filter.vagonNumber))
            {
                query = query.Where(r =>
                r.Weighings.Any(w => w.VagonNumber == filter.vagonNumber));
            }

            var receipts = await query
                .Select(r => new ReceiptDto
                {
                    Id = r.Id,
                    DateTime = r.DateTime,
                    TypeWeighng = r.TypeWeighng,
                    Operator = r.Operator
                })
                .ToListAsync();

            return new ServiceResult<List<ReceiptDto>>
            {
                Success = true,
                Data = receipts
            };
        }

    }
}
