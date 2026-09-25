using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using System;
using System.IO;
using System.Text.Json;

namespace PromVesClient.Service
{
    public class SerialPortBoardService
    {
        private readonly ILogger<SerialPortBoardService> _logger;

        private readonly string _configurationPath;
        private readonly string _settingsPath;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public SerialPortBoardService(
            ILogger<SerialPortBoardService> logger)
        {
            _logger = logger;

            _configurationPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration");

            _settingsPath = Path.Combine(
                _configurationPath,
                "SerialPortsBoards.json");
        }
        //метод загрузки настроек COM-порта табло
        public ServiceResult<SerialPortBoardConfiguration> Load()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    return ServiceResult<SerialPortBoardConfiguration>.Ok(
                        new SerialPortBoardConfiguration());
                }

                string json = File.ReadAllText(_settingsPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return ServiceResult<SerialPortBoardConfiguration>.Ok(
                        new SerialPortBoardConfiguration());
                }

                var configuration =
                    JsonSerializer.Deserialize<SerialPortBoardConfiguration>(
                        json,
                        _jsonOptions);

                if (configuration == null)
                {
                    return ServiceResult<SerialPortBoardConfiguration>.Fail(
                        "Не удалось загрузить настройки COM-порта табло.");
                }

                return ServiceResult<SerialPortBoardConfiguration>.Ok(
                    configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка загрузки SerialPortsBoards.json.");

                return ServiceResult<SerialPortBoardConfiguration>.Fail(
                    "Не удалось загрузить настройки COM-порта табло.");
            }
        }

        //метод сохранения настроек COM-порта табло
        public ServiceResult Save(
            SerialPortBoardConfiguration configuration)
        {
            try
            {
                Directory.CreateDirectory(_configurationPath);

                string json = JsonSerializer.Serialize(
                    configuration,
                    _jsonOptions);

                File.WriteAllText(
                    _settingsPath,
                    json);

                _logger.LogInformation(
                    "Настройки SerialPortsBoards сохранены.");

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка сохранения SerialPortsBoards.json.");

                return ServiceResult.Fail(
                    "Не удалось сохранить настройки COM-порта табло.");
            }
        }
    }
}
