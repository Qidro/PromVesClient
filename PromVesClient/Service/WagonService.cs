using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Service
{
    public class WagonService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ILogger<WagonService> _logger;

        public WagonService(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            ILogger<WagonService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        // Получение всех вагонов
        public async Task<ServiceResult<List<Wagon>>> GetAllAsync()
        {
            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var wagons = await db.Wagons
                    .OrderBy(w => w.Number)
                    .ToListAsync();

                return ServiceResult<List<Wagon>>.Ok(wagons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка вагонов.");

                return ServiceResult<List<Wagon>>.Fail(
                    "Не удалось загрузить список вагонов.");
            }
        }

        // Получение вагона по Id
        public async Task<ServiceResult<Wagon>> GetByIdAsync(Guid id)
        {
            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var wagon = await db.Wagons
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (wagon == null)
                {
                    return ServiceResult<Wagon>.Fail(
                        "Вагон не найден.");
                }

                return ServiceResult<Wagon>.Ok(wagon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при получении вагона {WagonId}.", id);

                return ServiceResult<Wagon>.Fail(
                    "Не удалось получить данные вагона.");
            }
        }

        // Добавление нового вагона
        public async Task<ServiceResult> CreateAsync(
            string number,
            decimal tareWeight)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(number))
                {
                    return ServiceResult.Fail(
                        "Номер вагона не может быть пустым.");
                }

                if (tareWeight < 0)
                {
                    return ServiceResult.Fail(
                        "Тара не может быть отрицательной.");
                }

                await using var db = await _dbContextFactory.CreateDbContextAsync();

                // Проверяем, существует ли такой номер
                var exists = await db.Wagons
                    .AnyAsync(w => w.Number == number);

                if (exists)
                {
                    return ServiceResult.Fail(
                        "Вагон с таким номером уже существует.");
                }

                var wagon = new Wagon
                {
                    Id = Guid.NewGuid(),
                    Number = number,
                    TareWeight = tareWeight,
                    IsActive = true
                };

                await db.Wagons.AddAsync(wagon);
                await db.SaveChangesAsync();

                _logger.LogInformation(
                    "Добавлен вагон {WagonNumber}.", number);

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при добавлении вагона {WagonNumber}.",
                    number);

                return ServiceResult.Fail(
                    "Не удалось добавить вагон.");
            }
        }

        // Изменение вагона
        public async Task<ServiceResult> UpdateAsync(
            Guid id,
            string number,
            decimal tareWeight)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(number))
                {
                    return ServiceResult.Fail(
                        "Номер вагона не может быть пустым.");
                }

                if (tareWeight < 0)
                {
                    return ServiceResult.Fail(
                        "Тара не может быть отрицательной.");
                }

                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var wagon = await db.Wagons
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (wagon == null)
                {
                    return ServiceResult.Fail(
                        "Вагон не найден.");
                }

                // Проверяем, не занят ли номер другим вагоном
                var numberExists = await db.Wagons
                    .AnyAsync(w =>
                        w.Number == number &&
                        w.Id != id);

                if (numberExists)
                {
                    return ServiceResult.Fail(
                        "Другой вагон уже имеет такой номер.");
                }

                wagon.Number = number;
                wagon.TareWeight = tareWeight;

                await db.SaveChangesAsync();

                _logger.LogInformation(
                    "Изменен вагон {WagonId}.", id);

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при изменении вагона {WagonId}.", id);

                return ServiceResult.Fail(
                    "Не удалось изменить вагон.");
            }
        }

        // Активация / деактивация вагона
        public async Task<ServiceResult> SetActiveAsync(
            Guid id,
            bool isActive)
        {
            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var wagon = await db.Wagons
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (wagon == null)
                {
                    return ServiceResult.Fail(
                        "Вагон не найден.");
                }

                wagon.IsActive = isActive;

                await db.SaveChangesAsync();

                _logger.LogInformation(
                    "Для вагона {WagonId} установлен статус {Status}.",
                    id,
                    isActive ? "Активный" : "Неактивный");

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка изменения статуса вагона {WagonId}.", id);

                return ServiceResult.Fail(
                    "Не удалось изменить статус вагона.");
            }
        }
    }
}
