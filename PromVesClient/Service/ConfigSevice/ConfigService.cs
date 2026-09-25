using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PromVesClient.Service.ConfigSevice
{
    public class ConfigService
    {
        private readonly ILogger<ComPortService> _logger;
        //private readonly SerialPort _serialPort = new();
        private readonly string _configurationProtocol;
        // Путь к рабочему файлу количество портов/серверов на сервере
        private readonly string _configurationPort;
        private readonly ComPortService _comPortService;
        //конвертирует в файле json в формат enwy
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
    {
        new JsonStringEnumConverter()
    }
        };
        public ConfigService(ILogger<ComPortService> logger, ComPortService _comPortService)
        { 
            
        }
        //расчет количество графиков
        public async Task<ServiceResult<int>> GetGraphsCountAsync()
        {
            return ServiceResult<int>.Ok(1);
        }
        //получение протокола взвешивания
        public async Task<ServiceResult<string>> GetProtocolAsync()
        {
            string _serverSettingsPath = @"C:\PromVesNew\PromVesServer\ConfigPort.json";
            if (!File.Exists(_serverSettingsPath))
            {
                _logger.LogWarning(
                    "Файл настроек {Path} не найден. Загружаются настройки по умолчанию.",
                    _serverSettingsPath);

                return ServiceResult<string>.Fail("Файл настроек статического взвешивания не найден");
            }

            string json = File.ReadAllText(_serverSettingsPath);

            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning(
                    "Файл настроек {Path} пустой. Загружаются настройки по умолчанию.",
                    _serverSettingsPath);
            }

            var configuration = JsonSerializer.Deserialize<SerialPortConfiguration>(json, _jsonOptions);

            if (configuration == null)
            {
                _logger.LogWarning(
                    "Не удалось десериализовать файл настроек. Загружаются настройки по умолчанию.");
            }
            return ServiceResult<string>.Ok("");
        }

        //получение количества портов/серверов
        public async Task<ServiceResult<int>> GetGraphsCountAsync(string protocolName)
        {
            try
            {
                //путь к файлу
                string _serverSettingsPath;
                //выбор файла в зависимости от протокола обмена данных
                if (protocolName == "ModbusTcp")
                {
                    _serverSettingsPath = @"C:\PromVesNew\PromVesServer\Configuration\ConfigModbusTcp.json";
                }
                else
                {
                    _serverSettingsPath = @"C:\PromVesNew\PromVesServer\Configuration\ConfigPort.json";
                }
                //проверка есть ли файл
                if (!File.Exists(_serverSettingsPath))
                {
                    _logger.LogError(
                        "Файл настроек {Path} не найден.",
                        _serverSettingsPath);

                    return ServiceResult<int>.Fail("Файл настроек статического взвешивания не найден");
                }
                //чтение файла
                string json = File.ReadAllText(_serverSettingsPath);
                //проверка, что файл пустой
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning(
                        "Файл настроек {Path}",
                        _serverSettingsPath);
                    return ServiceResult<int>.Fail("Файл настроек статического взвешивания пустой");
                }

                var configuration = JsonSerializer.Deserialize<SerialPortConfiguration>(json, _jsonOptions);

                if (configuration == null)
                {
                    _logger.LogWarning(
                        "Не удалось десериализовать файл настроек.");
                    return ServiceResult<int>.Fail("Не удалось десериализовать файл настроек");
                }
                return ServiceResult<int>.Ok(configuration.SerialPorts.Count);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Ошибка десериализации файла настроек.");

                return ServiceResult<int>.Fail(
                    "Файл настроек поврежден.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка чтения файла настроек.");

                return ServiceResult<int>.Fail(
                    "Не удалось прочитать файл настроек.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex,
                    "Нет доступа к файлу настроек.");

                return ServiceResult<int>.Fail(
                    "Нет доступа к файлу настроек.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при загрузке настроек.");

                return ServiceResult<int>.Fail(
                    "Не удалось загрузить настройки.");
            }
            //return ServiceResult<int>.Ok(1);
        }
    }
}
