using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PromVesClient.Service
{

    public class ComPortService
    {
        private readonly ILogger<ComPortService> _logger;
        private readonly SerialPort _serialPort = new();
        private readonly string _configurationPath;
        private readonly string _defaultSettingsPath;
        // Путь к рабочему файлу на сервере
        private readonly string _serverSettingsPath;
        //конвертирует в файле json в формат enwy
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
    {
        new JsonStringEnumConverter()
    }
        };
       
        public ComPortService(ILogger<ComPortService> logger)
        {
            _logger = logger;

            _configurationPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration");

            _serverSettingsPath = @"C:\PromVesNew\PromVesServer\Configuration\ConfigPort.json";

            _defaultSettingsPath = Path.Combine(
                _configurationPath,
                "defaultSettings.json");
        }


        /// Загружает настройки COM-порта из файла.
        public ServiceResult<SerialPortConfiguration> Load()
        {
            try
            {
                if (!File.Exists(_serverSettingsPath))
                {
                    _logger.LogWarning(
                        "Файл настроек {Path} не найден. Загружаются настройки по умолчанию.",
                        _serverSettingsPath);

                    return LoadDefaults();
                }

                string json = File.ReadAllText(_serverSettingsPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning(
                        "Файл настроек {Path} пустой. Загружаются настройки по умолчанию.",
                        _serverSettingsPath);

                    return LoadDefaults();
                }

                var configuration = JsonSerializer.Deserialize<SerialPortConfiguration>(
     json,
     _jsonOptions);

                if (configuration == null)
                {
                    _logger.LogWarning(
                        "Не удалось десериализовать файл настроек. Загружаются настройки по умолчанию.");

                    return LoadDefaults();
                }

                return ServiceResult<SerialPortConfiguration>.Ok(configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Ошибка десериализации файла настроек.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Файл настроек поврежден.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка чтения файла настроек.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Не удалось прочитать файл настроек.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex,
                    "Нет доступа к файлу настроек.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Нет доступа к файлу настроек.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при загрузке настроек.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Не удалось загрузить настройки.");
            }
        }

        /// Загружает настройки COM-порта по умолчанию из файла.
        public ServiceResult<SerialPortConfiguration> LoadDefaults()
        {
            try
            {
                string json = File.ReadAllText(_defaultSettingsPath);

                var configuration = JsonSerializer.Deserialize<SerialPortConfiguration>(
                    json,
                    _jsonOptions);

                if (configuration == null)
                {
                    _logger.LogWarning(
                        "Не удалось загрузить настройки по умолчанию из файла {Path}.",
                        _defaultSettingsPath);

                    return ServiceResult<SerialPortConfiguration>.Fail(
                        "Файл настроек по умолчанию поврежден.");
                }

                return ServiceResult<SerialPortConfiguration>.Ok(configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Ошибка десериализации файла настроек по умолчанию.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Файл настроек по умолчанию поврежден.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка чтения файла настроек по умолчанию.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Не удалось прочитать файл настроек по умолчанию.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex,
                    "Нет доступа к файлу настроек по умолчанию.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Нет доступа к файлу настроек по умолчанию.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при загрузке настроек по умолчанию.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Не удалось загрузить настройки по умолчанию.");
            }
        }

        /// Сохраняет настройки COM-порта в файл.
        public ServiceResult Save(SerialPortConfiguration configuration)
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    configuration,
                    _jsonOptions);

                File.WriteAllText(_serverSettingsPath, json);

                return ServiceResult.Ok();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Ошибка сериализации настроек COM-портов.");

                return ServiceResult.Fail(
                    "Не удалось подготовить настройки к сохранению.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка записи файла настроек {Path}.",
                    _serverSettingsPath);

                return ServiceResult.Fail(
                    "Не удалось сохранить настройки.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex,
                    "Нет доступа к файлу настроек {Path}.",
                    _serverSettingsPath);

                return ServiceResult.Fail(
                    "Нет доступа к файлу настроек.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при сохранении настроек.");

                return ServiceResult.Fail(
                    "Не удалось сохранить настройки.");
            }
        }
        /// Восстанавливает настройки COM-порта по умолчанию.
        public ServiceResult<SerialPortConfiguration> RestoreDefaults()
        {
            try
            {
                return LoadDefaults();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при загрузке настроек по умолчанию.");

                return ServiceResult<SerialPortConfiguration>.Fail(
                    "Не удалось загрузить настройки по умолчанию.");
            }
        }
        // Получает список доступных COM-портов.
        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames()
                             .OrderBy(port => port)
                             .ToArray();
        }

    }
}    

