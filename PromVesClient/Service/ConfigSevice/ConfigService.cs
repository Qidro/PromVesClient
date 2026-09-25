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
        public ConfigService(ILogger<ComPortService> logger, ComPortService comPortService)
        {
            _logger = logger;
            _comPortService = comPortService;
        }
        //расчет количество графиков
        public async Task<ServiceResult<int>> GetGraphsCountAsync()
        {
            //получаем протокол взвешивания
            var resultGetProtocol = await GetProtocolAsync();
            if (resultGetProtocol.Success == true)
            {
                //получаем количество графиков
                var resultGetGraphsCount = await GetGraphsCountAsync(resultGetProtocol.Data);
                if (resultGetGraphsCount.Success == true)
                {
                    return ServiceResult<int>.Ok(resultGetGraphsCount.Data);
                }
                else
                {
                    return ServiceResult<int>.Fail($"не удалось получить количества графиков взвешивания, причина: { resultGetGraphsCount.Message}");
                }
            }
            else
            {
                return ServiceResult<int>.Fail($"не удалось получить протокол взвешивания, причина: {resultGetProtocol.Message}");
            }
           // return ServiceResult<int>.Ok(1);
        }
        //получение протокола взвешивания
        private async Task<ServiceResult<string>> GetProtocolAsync()
        {
            //путь к файлу
            string _serverSettingsPath = @"C:\PromVesNew\PromVesServer\Configuration\GeneralConfigurator.json";
            //открытие файла
            if (!File.Exists(_serverSettingsPath))
            {
                _logger.LogError(
                    "Файл настроек {Path} не найден.",
                    _serverSettingsPath);

                return ServiceResult<string>.Fail("Файл настроек статического взвешивания не найден");
            }
            //чтение файла
            string json = await File.ReadAllTextAsync(_serverSettingsPath);
            //проверка, что он не пустой
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogError(
                    "Файл настроек {Path} пустой",
                    _serverSettingsPath);
            }
            //десереализуем
            var configuration = JsonSerializer.Deserialize<GeneralConfiguratorConfiguration>(json, _jsonOptions);

            if (configuration == null)
            {
                _logger.LogError(
                        "Не удалось десериализовать файл настроек: {Path}", _serverSettingsPath);
                return ServiceResult<string>.Fail("Не удалось десериализовать файл настроек");
            }
            return ServiceResult<string>.Ok(configuration.GeneralConfigurator.Protocol);
        }

        //получение количества портов/серверов
        private async Task<ServiceResult<int>> GetGraphsCountAsync(string protocolName)
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

                    return ServiceResult<int>.Fail("Файл настроек количества графиков статического взвешивания не найден");
                }
                //чтение файла
                string json = File.ReadAllText(_serverSettingsPath);
                //проверка, что файл пустой
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning(
                        "Файл настроек {Path}",
                        _serverSettingsPath);
                    return ServiceResult<int>.Fail("Файл настроек количества графиков статического взвешивания пустой");
                }
                if (protocolName == "ModbusTcp")
                {
                    var configuration = JsonSerializer.Deserialize<ModbusTcpConfiguration>(json, _jsonOptions);

                    if (configuration == null)
                    {
                        _logger.LogWarning(
                            "Не удалось десериализовать файл настроек количества графиков статического взвешивания");
                        return ServiceResult<int>.Fail("Не удалось десериализовать файл настроек количества графиков статического взвешивания");
                    }
                    return ServiceResult<int>.Ok(configuration.ModbusTcpSetting.Count);
                }
                else
                {
                    var configuration = JsonSerializer.Deserialize<SerialPortConfiguration>(json, _jsonOptions);

                    if (configuration == null)
                    {
                        _logger.LogWarning(
                            "Не удалось десериализовать файл настроек количества графиков статического взвешивания");
                        return ServiceResult<int>.Fail("Не удалось десериализовать файл настроек количества графиков статического взвешивания");
                    }
                    return ServiceResult<int>.Ok(configuration.SerialPorts.Count);
                }
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
