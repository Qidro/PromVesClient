using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using PromVesClient.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PromVesClient.Services
{
    public class ReceiptPrintSettingsService
    {
        private readonly ILogger<
            ReceiptPrintSettingsService> _logger;

        private readonly string _settingsDirectory;
        private readonly string _settingsPath;

        public ReceiptPrintSettingsService(ILogger<ReceiptPrintSettingsService> logger)
        {
            _logger = logger;


            _settingsDirectory = Path.Combine(AppContext.BaseDirectory, "Configuration");
            _settingsPath = Path.Combine(_settingsDirectory, "ReceiptPrintSettings.json");
        }

        // Метод загрузки настроек из json-файла
        public ServiceResult<ReceiptPrintSettings> Load()
        {
            try
            {
                if (!Directory.Exists(_settingsDirectory))
                {
                    Directory.CreateDirectory(_settingsDirectory);
                    _logger.LogInformation("Создан каталог настроек {Directory}", _settingsDirectory);
                }

                if (!File.Exists(_settingsPath))
                {
                    _logger.LogInformation("Файл настроек не найден. Создаются настройки по умолчанию.");

                    var defaultSettings = new ReceiptPrintSettings();

                    var saveResult = Save(defaultSettings);

                    if (!saveResult.Success)
                    {
                        return ServiceResult<ReceiptPrintSettings>.Fail(saveResult.Message!);
                    }

                    return ServiceResult<ReceiptPrintSettings>.Ok(defaultSettings);
                }

                var json = File.ReadAllText(_settingsPath);

                var settings = JsonSerializer.Deserialize<ReceiptPrintSettings>(json);

                if (settings == null)
                {
                    _logger.LogWarning("Не удалось десериализовать ReceiptPrintSettings.json");

                    return ServiceResult<ReceiptPrintSettings>.Fail("Файл настроек поврежден.");
                }

                return ServiceResult<ReceiptPrintSettings>.Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка загрузки настроек печати.");

                return ServiceResult<ReceiptPrintSettings>.Fail("Ошибка загрузки настроек.");
            }
        }

        // Метод сохранения настроек в json-файл
        public ServiceResult Save(ReceiptPrintSettings settings)
        {
            try
            {
                if (!Directory.Exists(_settingsDirectory))
                {
                    Directory.CreateDirectory(_settingsDirectory);
                }

                var json = JsonSerializer.Serialize(
                    settings,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_settingsPath, json);

                _logger.LogInformation("Настройки печати успешно сохранены.");

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения настроек печати.");

                return ServiceResult.Fail("Не удалось сохранить настройки.");
            }
        }
    }
}
