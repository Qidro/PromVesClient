using Microsoft.Extensions.Logging;
using PromVesClient.Models.ConfigurationModels;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PromVesClient.Service
{
    // Сервис отвечает за чтение и сохранение настроек COM-порта устройства PN.
    public class PNSerialPortService
    {
        private const string ConfigurationPath =
            @"C:\PromVesNew\PromVesServer\Configuration\ConfigPN.json";

        private readonly string _defaultSettingsPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "defaultPNSettings.json");

        private readonly ILogger<PNSerialPortService> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public PNSerialPortService(ILogger<PNSerialPortService> logger)
        {
            _logger = logger;
        }

        // Асинхронно загружает стандартные настройки PN из файла,
        // который поставляется вместе с клиентским приложением.
        public async Task<ServiceResult<PNSerialPortConfiguration>> LoadDefaultsAsync()
        {
            try
            {
                if (!File.Exists(_defaultSettingsPath))
                {
                    return ServiceResult<PNSerialPortConfiguration>.Fail(
                        $"Файл стандартных настроек PN не найден: {_defaultSettingsPath}");
                }

                // Файл используется только для чтения defaults и не изменяется
                // при сохранении рабочих настроек ConfigPN.json.
                string json = await File.ReadAllTextAsync(_defaultSettingsPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return ServiceResult<PNSerialPortConfiguration>.Fail(
                        "Файл стандартных настроек PN пуст.");
                }

                var configuration = JsonSerializer.Deserialize<PNSerialPortConfiguration>(
                    json,
                    _jsonOptions);

                if (configuration?.SerialPorts is null ||
                    configuration.SerialPorts.Count == 0)
                {
                    return ServiceResult<PNSerialPortConfiguration>.Fail(
                        "Файл стандартных настроек PN не содержит параметров COM-порта.");
                }

                return ServiceResult<PNSerialPortConfiguration>.Ok(configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Ошибка десериализации стандартных настроек PN {Path}.",
                    _defaultSettingsPath);
                return ServiceResult<PNSerialPortConfiguration>.Fail(
                    "Файл стандартных настроек PN имеет некорректный формат.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Ошибка чтения стандартных настроек PN {Path}.",
                    _defaultSettingsPath);
                return ServiceResult<PNSerialPortConfiguration>.Fail(
                    "Не удалось прочитать стандартные настройки PN.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex,
                    "Нет доступа к стандартным настройкам PN {Path}.",
                    _defaultSettingsPath);
                return ServiceResult<PNSerialPortConfiguration>.Fail(
                    "Нет доступа к стандартным настройкам PN.");
            }
        }

        // Асинхронно загружает рабочие настройки PN из ConfigPN.json.
        public async Task<ServiceResult<PNSerialPortConfiguration>> LoadAsync()
        {
            try
            {
                // Если файл отсутствует, форма получит понятное сообщение об ошибке,
                // а не исключение из файловой системы.
                if (!File.Exists(ConfigurationPath))
                {
                    return ServiceResult<PNSerialPortConfiguration>.Fail(
                        $"Файл настроек PN не найден: {ConfigurationPath}");
                }

                // Асинхронное чтение не блокирует UI-поток во время обращения к диску.
                string json = await File.ReadAllTextAsync(ConfigurationPath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return ServiceResult<PNSerialPortConfiguration>.Fail(
                        "Файл настроек PN пуст.");
                }

                var configuration = JsonSerializer.Deserialize<PNSerialPortConfiguration>(
                    json,
                    _jsonOptions);

                if (configuration?.SerialPorts is null ||
                    configuration.SerialPorts.Count == 0)
                {
                    return ServiceResult<PNSerialPortConfiguration>.Fail(
                        "Файл настроек PN не содержит параметров COM-порта.");
                }

                return ServiceResult<PNSerialPortConfiguration>.Ok(configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации файла настроек PN {Path}.", ConfigurationPath);
                return ServiceResult<PNSerialPortConfiguration>.Fail(
                    "Файл настроек PN имеет некорректный формат.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Ошибка чтения файла настроек PN {Path}.", ConfigurationPath);
                return ServiceResult<PNSerialPortConfiguration>.Fail(
                    "Не удалось прочитать файл настроек PN.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Нет доступа к файлу настроек PN {Path}.", ConfigurationPath);
                return ServiceResult<PNSerialPortConfiguration>.Fail(
                    "Нет доступа к файлу настроек PN.");
            }
        }

        // Асинхронно сохраняет рабочие настройки PN в ConfigPN.json.
        public async Task<ServiceResult> SaveAsync(PNSerialPortConfiguration configuration)
        {
            try
            {
                // В файл нельзя записывать пустую конфигурацию,
                // поскольку сервер не сможет определить COM-порт PN.
                if (configuration is null ||
                    configuration.SerialPorts is null ||
                    configuration.SerialPorts.Count == 0)
                {
                    return ServiceResult.Fail(
                        "Не заданы настройки COM-порта PN.");
                }

                string json = JsonSerializer.Serialize(configuration, _jsonOptions);

                // Асинхронная запись позволяет форме оставаться отзывчивой,
                // если файл временно занят или находится на медленном диске.
                await File.WriteAllTextAsync(ConfigurationPath, json);

                return ServiceResult.Ok();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка сериализации настроек PN.");
                return ServiceResult.Fail(
                    "Не удалось подготовить настройки PN к сохранению.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Ошибка записи файла настроек PN {Path}.", ConfigurationPath);
                return ServiceResult.Fail(
                    "Не удалось сохранить настройки PN.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Нет доступа к файлу настроек PN {Path}.", ConfigurationPath);
                return ServiceResult.Fail(
                    "Нет доступа к файлу настроек PN.");
            }
        }
    }
}
