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

            _serverSettingsPath = @"C:\PromVesNew\PromVesServer\ConfigPort.json";

            _defaultSettingsPath = Path.Combine(
                _configurationPath,
                "defaultSettings.json");
        }


        /// <summary>
        /// Загружает текущие настройки.
        /// Если файла нет — создает его из defaultSettings.json.
        /// </summary>
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

        /// <summary>
        /// Загружает настройки по умолчанию.
        /// </summary>
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

        /// Сохраняет настройки.
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
        /// <summary>
        /// Восстанавливает настройки по умолчанию.
        /// </summary>
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
        /// <summary>
        /// Возвращает список доступных COM-портов.
        /// </summary>
        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames()
                             .OrderBy(port => port)
                             .ToArray();
        }

        // if (_comPortService.IsOpen)
        //{
        //  ...
        //}
        // благодоря этому Теперь любая форма сможет написать
        public bool IsOpen
        {
            get
            {
                return _serialPort.IsOpen;
            }
        }
        // открытие порта
        public ServiceResult Open(SerialPortSettings settings)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                _serialPort.PortName = settings.PortName;
                _serialPort.BaudRate = settings.BaudRate;
                _serialPort.DataBits = settings.DataBits;
                _serialPort.Parity = settings.Parity;
                _serialPort.StopBits = settings.StopBits;
                _serialPort.Handshake = settings.Handshake;

                _serialPort.Open();

                _logger.LogInformation(
                    "COM-порт {PortName} успешно открыт.",
                    settings.PortName);

                return ServiceResult.Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex,
                    "Нет доступа к COM-порту {PortName}.",
                    settings.PortName);

                return ServiceResult.Fail("Нет доступа к COM-порту.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка ввода-вывода при открытии COM-порта {PortName}.",
                    settings.PortName);

                return ServiceResult.Fail("Ошибка ввода-вывода при открытии COM-порта.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Ошибка открытия COM-порта {PortName}.",
                    settings.PortName);

                return ServiceResult.Fail("COM-порт уже открыт или находится в недопустимом состоянии.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex,
                    "Некорректные параметры COM-порта {PortName}.",
                    settings.PortName);

                return ServiceResult.Fail("Некорректные параметры COM-порта.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при открытии COM-порта {PortName}.",
                    settings.PortName);

                return ServiceResult.Fail("Не удалось открыть COM-порт.");
            }
        }
        // закрытие
        public ServiceResult Close()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();

                    _logger.LogInformation(
                        "COM-порт {PortName} успешно закрыт.",
                        _serialPort.PortName);
                }
                else
                {
                    _logger.LogInformation(
                        "Попытка закрыть COM-порт, но он уже закрыт.");
                }

                return ServiceResult.Ok();
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка ввода-вывода при закрытии COM-порта.");

                return ServiceResult.Fail("Ошибка при закрытии COM-порта.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Ошибка закрытия COM-порта.");

                return ServiceResult.Fail("Не удалось закрыть COM-порт.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при закрытии COM-порта.");

                return ServiceResult.Fail("Не удалось закрыть COM-порт.");
            }
        }
        //запись
        public ServiceResult Write(string text)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _logger.LogWarning(
                        "Попытка записи в COM-порт, который не открыт.");

                    return ServiceResult.Fail("COM-порт не открыт.");
                }

                _serialPort.Write(text);

                _logger.LogInformation(
                    "Данные успешно отправлены в COM-порт {PortName}.",
                    _serialPort.PortName);

                return ServiceResult.Ok();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex,
                    "Превышено время ожидания при записи в COM-порт {PortName}.",
                    _serialPort.PortName);

                return ServiceResult.Fail("Превышено время ожидания при записи в COM-порт.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Ошибка записи в COM-порт {PortName}.",
                    _serialPort.PortName);

                return ServiceResult.Fail("COM-порт недоступен.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка ввода-вывода при записи в COM-порт {PortName}.",
                    _serialPort.PortName);

                return ServiceResult.Fail("Ошибка ввода-вывода при записи в COM-порт.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при записи в COM-порт {PortName}.",
                    _serialPort.PortName);

                return ServiceResult.Fail("Не удалось выполнить запись в COM-порт.");
            }
        }
        //чтение
        public ServiceResult<string> ReadLine()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _logger.LogWarning(
                        "Попытка чтения из COM-порта, который не открыт.");

                    return ServiceResult<string>.Fail("COM-порт не открыт.");
                }

                string data = _serialPort.ReadLine();

                _logger.LogInformation(
                    "Данные успешно получены из COM-порта {PortName}.",
                    _serialPort.PortName);

                return ServiceResult<string>.Ok(data);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex,
                    "Превышено время ожидания при чтении из COM-порта {PortName}.",
                    _serialPort.PortName);

                return ServiceResult<string>.Fail(
                    "Превышено время ожидания при чтении из COM-порта.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Ошибка чтения из COM-порта {PortName}.",
                    _serialPort.PortName);

                return ServiceResult<string>.Fail("COM-порт недоступен.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка ввода-вывода при чтении из COM-порта {PortName}.",
                    _serialPort.PortName);

                return ServiceResult<string>.Fail(
                    "Ошибка ввода-вывода при чтении из COM-порта.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Неизвестная ошибка при чтении из COM-порта {PortName}.",
                    _serialPort.PortName);

                return ServiceResult<string>.Fail(
                    "Не удалось прочитать данные из COM-порта.");
            }
        }

    }
}    

