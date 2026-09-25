using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using System;
using System.IO;
using System.Text.Json;

namespace PromVesClient.Service
{
    public class ModbusTcpService
    {
        private readonly ILogger<ModbusTcpService> _logger;

        private readonly string _configurationPath;
        private readonly string _serverSettingsPath;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public ModbusTcpService(ILogger<ModbusTcpService> logger)
        {
            _logger = logger;

            _configurationPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration");

            _serverSettingsPath = Path.Combine(
                _configurationPath,
                "ConfigModbusTcp.json");
        }

        // загружает настройки MOXA из файла ConfigModbusTcp.json
        public ServiceResult<ModbusTcpConfiguration> Load()
        {
            try
            {
                if (!File.Exists(_serverSettingsPath))
                {
                    _logger.LogWarning(
                        "Файл настроек MOXA {Path} не найден.",
                        _serverSettingsPath);

                    return LoadDefaults();
                }

                string json = File.ReadAllText(_serverSettingsPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning(
                        "Файл настроек MOXA {Path} пустой.",
                        _serverSettingsPath);

                    return LoadDefaults();
                }

                var configuration =
                    JsonSerializer.Deserialize<ModbusTcpConfiguration>(
                        json,
                        _jsonOptions);

                if (configuration == null)
                {
                    _logger.LogWarning(
                        "Не удалось десериализовать настройки MOXA.");

                    return LoadDefaults();
                }

                return ServiceResult<ModbusTcpConfiguration>.Ok(
                    configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка десериализации ConfigModbusTcp.json.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Файл настроек MOXA поврежден.");
            }
            catch (IOException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка чтения файла настроек MOXA.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Не удалось прочитать настройки MOXA.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Нет доступа к файлу настроек MOXA.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Нет доступа к файлу настроек MOXA.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Неизвестная ошибка при загрузке настроек MOXA.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Не удалось загрузить настройки MOXA.");
            }
        }

        // загружает настройки MOXA по умолчанию из файла defaultConfigModbusTcp.json
        public ServiceResult<ModbusTcpConfiguration> LoadDefaults()
        {
            try
            {
                string defaultSettingsPath = Path.Combine(
                    _configurationPath,
                    "defaultConfigModbusTcp.json");

                if (!File.Exists(defaultSettingsPath))
                {
                    _logger.LogWarning(
                        "Файл настроек MOXA по умолчанию {Path} не найден.",
                        defaultSettingsPath);

                    return ServiceResult<ModbusTcpConfiguration>.Fail(
                        "Файл настроек MOXA по умолчанию не найден.");
                }

                string json = File.ReadAllText(defaultSettingsPath);

                var configuration =
                    JsonSerializer.Deserialize<ModbusTcpConfiguration>(
                        json,
                        _jsonOptions);

                if (configuration == null)
                {
                    _logger.LogWarning(
                        "Не удалось загрузить настройки MOXA по умолчанию.");

                    return ServiceResult<ModbusTcpConfiguration>.Fail(
                        "Файл настроек MOXA по умолчанию поврежден.");
                }

                return ServiceResult<ModbusTcpConfiguration>.Ok(
                    configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка десериализации настроек MOXA по умолчанию.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Файл настроек MOXA по умолчанию поврежден.");
            }
            catch (IOException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка чтения настроек MOXA по умолчанию.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Не удалось прочитать настройки MOXA по умолчанию.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Нет доступа к настройкам MOXA по умолчанию.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Нет доступа к файлу настроек MOXA по умолчанию.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Неизвестная ошибка при загрузке настроек MOXA по умолчанию.");

                return ServiceResult<ModbusTcpConfiguration>.Fail(
                    "Не удалось загрузить настройки MOXA по умолчанию.");
            }
        }

        // сохраняет настройки MOXA в файл ConfigModbusTcp.json
        public ServiceResult Save(ModbusTcpConfiguration configuration)
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    configuration,
                    _jsonOptions);

                Directory.CreateDirectory(_configurationPath);

                File.WriteAllText(
                    _serverSettingsPath,
                    json);

                _logger.LogInformation(
                    "Настройки MOXA успешно сохранены в {Path}.",
                    _serverSettingsPath);

                return ServiceResult.Ok();
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка сериализации настроек MOXA.");

                return ServiceResult.Fail(
                    "Не удалось подготовить настройки MOXA к сохранению.");
            }
            catch (IOException ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка записи файла настроек MOXA {Path}.",
                    _serverSettingsPath);

                return ServiceResult.Fail(
                    "Не удалось сохранить настройки MOXA.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Нет доступа к файлу настроек MOXA {Path}.",
                    _serverSettingsPath);

                return ServiceResult.Fail(
                    "Нет доступа к файлу настроек MOXA.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Неизвестная ошибка при сохранении настроек MOXA.");

                return ServiceResult.Fail(
                    "Не удалось сохранить настройки MOXA.");
            }
        }
    }
}
