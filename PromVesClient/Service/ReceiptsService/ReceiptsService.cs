using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
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

    }
}
