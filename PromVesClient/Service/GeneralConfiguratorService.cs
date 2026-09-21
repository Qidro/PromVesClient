using Microsoft.Extensions.Logging;
using PromVesClient.Models;
using System;
using System.IO;
using System.Text.Json;

namespace PromVesClient.Service
{
    public class GeneralConfiguratorService
    {
        private readonly ILogger<GeneralConfiguratorService> _logger;

        private readonly string _configurationPath;
        private readonly string _settingsPath;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public GeneralConfiguratorService(
            ILogger<GeneralConfiguratorService> logger)
        {
            _logger = logger;

            _configurationPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration");

            _settingsPath = Path.Combine(
                _configurationPath,
                "GeneralConfigurator.json");
        }

        public ServiceResult<GeneralConfiguratorConfiguration> Load()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    return ServiceResult<GeneralConfiguratorConfiguration>.Ok(
                        new GeneralConfiguratorConfiguration());
                }

                string json = File.ReadAllText(_settingsPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return ServiceResult<GeneralConfiguratorConfiguration>.Ok(
                        new GeneralConfiguratorConfiguration());
                }

                var configuration =
                    JsonSerializer.Deserialize<GeneralConfiguratorConfiguration>(
                        json,
                        _jsonOptions);

                if (configuration == null)
                {
                    return ServiceResult<GeneralConfiguratorConfiguration>.Fail(
                        "Не удалось загрузить общие настройки.");
                }

                return ServiceResult<GeneralConfiguratorConfiguration>.Ok(
                    configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка загрузки GeneralConfigurator.json.");

                return ServiceResult<GeneralConfiguratorConfiguration>.Fail(
                    "Не удалось загрузить общие настройки.");
            }
        }

        public ServiceResult Save(
            GeneralConfiguratorConfiguration configuration)
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
                    "GeneralConfigurator сохранен.");

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка сохранения GeneralConfigurator.json.");

                return ServiceResult.Fail(
                    "Не удалось сохранить общие настройки.");
            }
        }
    }
}