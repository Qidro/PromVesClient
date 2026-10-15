using Microsoft.Extensions.Logging;
using PromVesClient.Models.ConfigurationModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PromVesClient.Service.PNCalibrationService
{
    public class CalibrationService
    {
        private readonly ILogger<CalibrationService> _logger;
        //private readonly SerialPort _serialPort = new();
        private readonly string _configurationProtocol;
        // Путь к рабочему файлу количество портов/серверов на сервере
        private readonly string _configurationPort;
        //конвертирует в файле json в формат enwy
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
            {
            new JsonStringEnumConverter()
            }
        };
        public CalibrationService(ILogger<CalibrationService> logger)
        {
            _logger = logger;
        }

        // Получение настроек протокола взвешивания
        public async Task<ServiceResult<List<PNSetting>>> GetSettingCalibration()
        {
            const string settingsPath =
                @"C:\PromVesNew\PromVesServer\Configuration\ConfigСalibration.json";

            // Проверяем наличие файла
            if (!File.Exists(settingsPath))
            {
                _logger.LogError(
                    "Файл настроек не найден: {Path}",
                    settingsPath);

                return ServiceResult<List<PNSetting>>.Fail(
                    "Файл настроек статического взвешивания не найден.");
            }

            try
            {
                // Читаем JSON асинхронно
                string json = await File.ReadAllTextAsync(settingsPath);

                // Проверяем, что файл не пустой
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogError(
                        "Файл настроек пуст: {Path}",
                        settingsPath);

                    return ServiceResult<List<PNSetting>>.Fail(
                        "Файл настроек пуст.");
                }

                // Десериализуем JSON в объект конфигурации
                PNConfiguration? configuration =
                    JsonSerializer.Deserialize<PNConfiguration>(
                        json,
                        _jsonOptions);

                // Проверяем результат десериализации
                if (configuration?.PNSetting is null)
                {
                    _logger.LogError(
                        "Не удалось получить настройки из файла: {Path}",
                        settingsPath);

                    return ServiceResult<List<PNSetting>>.Fail(
                        "Не удалось прочитать настройки протокола взвешивания.");
                }

                // Возвращаем список настроек
                return ServiceResult<List<PNSetting>>.Ok(
                    configuration.PNSetting);
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка десериализации JSON-файла: {Path}",
                    settingsPath);

                return ServiceResult<List<PNSetting>>.Fail(
                    "Файл настроек имеет некорректный формат.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Нет доступа к файлу настроек: {Path}",
                    settingsPath);

                return ServiceResult<List<PNSetting>>.Fail(
                    "Нет доступа к файлу настроек.");
            }
            catch (IOException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка чтения файла настроек: {Path}",
                    settingsPath);

                return ServiceResult<List<PNSetting>>.Fail(
                    "Не удалось прочитать файл настроек.");
            }
        }
    }
}
