using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using PromVesClient.DTO;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            try
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
            catch (TimeoutException ex)
            {
                _logger.LogError("Привышенно время ожидания ответа: "+ ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("БД не отвечает, причина: " + ex.Message);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError("Ошибка сервера БД: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("Ошибка сервера БД: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Неизвестная ошибка БД: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("Неизвестная ошибка БД: " + ex.Message);
            }
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
            try
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
            catch (TimeoutException ex)
            {
                _logger.LogError("Привышенно время ожидания ответа: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("БД не отвечает, причина: " + ex.Message);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError("Ошибка сервера БД: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("Ошибка сервера БД: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Неизвестная ошибка БД: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("Неизвестная ошибка БД: " + ex.Message);
            }

        }
        //метод предназначен для поиска взвешиваний с квитанции
        public async Task<ServiceResult<List<CardsDto>>> GetCardsAsync(Guid IdReceipt)
        {
            try
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
            catch (TimeoutException ex)
            {
                _logger.LogError("Привышенно время ожидания ответа: " + ex.Message);
                return ServiceResult<List<CardsDto>>.Fail("БД не отвечает, причина: " + ex.Message);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError("Ошибка сервера БД: " + ex.Message);
                return ServiceResult<List<CardsDto>>.Fail("Ошибка сервера БД: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Неизвестная ошибка БД: " + ex.Message);
                return ServiceResult<List<CardsDto>>.Fail("Неизвестная ошибка БД: " + ex.Message);
            }
            
        }
        //метод получения квитанций с помощью фильтра
        public async Task<ServiceResult<List<ReceiptDto>>> GetReceiptFilter(SearchReceiptDto filter)
        {
            try 
            {
                //AsQueryable подчеркивает, что далее запрос будет строиться динамически (добавляться)
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
                        DateTime = DateTime.SpecifyKind(r.DateTime, DateTimeKind.Utc).ToLocalTime(),
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
            catch (TimeoutException ex)
            {
                _logger.LogError("Привышенно время ожидания ответа: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("БД не отвечает, причина: " + ex.Message);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError("Ошибка сервера БД: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("Ошибка сервера БД: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Неизвестная ошибка БД: " + ex.Message);
                return ServiceResult<List<ReceiptDto>>.Fail("Неизвестная ошибка БД: " + ex.Message);
            }
            
        }

        public async Task<ServiceResult> deletingCard(Guid IdWeighing)
        {
            try
            {
                var weighing = await _dbContext.Weighings.FindAsync(IdWeighing);

                if (weighing == null)
                {
                    return ServiceResult.Fail("Квитанция не найдена.");
                }

                _dbContext.Weighings.Remove(weighing);
                await _dbContext.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления квитанции");
                return ServiceResult.Fail("Ошибка удаления.");
            }
        }
        //удаление квитанции
        public async Task<ServiceResult> deletingReceipt(Guid IdCard)
        {
            try
            {
                var receipt = await _dbContext.Receipts.FindAsync(IdCard);

                if (receipt == null)
                {
                    return ServiceResult.Fail("Квитанция не найдена.");
                }

                _dbContext.Receipts.Remove(receipt);
                await _dbContext.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления квитанции");
                return ServiceResult.Fail("Ошибка удаления.");
            }
        }

    }
}
