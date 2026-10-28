using Microsoft.Extensions.Logging;
using PromVesClient.Models.ConfigurationModels;
using System.Text.Json;

namespace PromVesClient.Service
{
    public class CameraService
    {
        private readonly ILogger<CameraService> _logger;
        private readonly string _serverSettingsPath = @"C:\PromVesNew\PromVesServer\Configuration\ConfigCameras.json";
        //private readonly string _serverSettingsPathWithoutExtension = @"C:\PromVesNew\PromVesServer\Configuration\ConfigCameras";
        private readonly string _defaultSettingsPath;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public CameraService(ILogger<CameraService> logger)
        {
            _logger = logger;
            _defaultSettingsPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration",
                "defaultSettingCameras.json");
        }

        // Загружает рабочую конфигурацию камер с сервера.
        public async Task<ServiceResult<CameraConfiguration>> LoadAsync()
        {
            if (!File.Exists(_serverSettingsPath))
            {
                return ServiceResult<CameraConfiguration>.Fail(
                    "Файл настроек камер не найден.");
            }

            return await LoadFromFileAsync(
                _serverSettingsPath,
                "Файл настроек камер");
        }

        // Загружает локальные настройки по умолчанию, не изменяя серверный файл.
        public async Task<ServiceResult<CameraConfiguration>> LoadDefaultsAsync()
        {
            return await LoadFromFileAsync(
                _defaultSettingsPath,
                "Файл настроек камер по умолчанию");
        }

        // Сохраняет конфигурацию именно в файл, который использует PromVesServer.
        public async Task<ServiceResult> SaveAsync(CameraConfiguration configuration)
        {
            try
            {
                string json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(_serverSettingsPath, json);
                return ServiceResult.Ok();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка сериализации настроек камер.");
                return ServiceResult.Fail("Не удалось подготовить настройки камер к сохранению.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Ошибка записи настроек камер.");
                return ServiceResult.Fail("Не удалось записать настройки камер.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Нет доступа к файлу настроек камер.");
                return ServiceResult.Fail("Нет доступа к файлу настроек камер.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при сохранении настроек камер.");
                return ServiceResult.Fail("Не удалось сохранить настройки камер.");
            }
        }

        private async Task<ServiceResult<CameraConfiguration>> LoadFromFileAsync(
            string path,
            string fileDescription)
        {
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogWarning("{Description} {Path} не найден.", fileDescription, path);
                    return ServiceResult<CameraConfiguration>.Fail($"{fileDescription} не найден.");
                }

                string json = await File.ReadAllTextAsync(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning("{Description} {Path} пустой.", fileDescription, path);
                    return ServiceResult<CameraConfiguration>.Fail($"{fileDescription} пустой.");
                }

                var configuration = JsonSerializer.Deserialize<CameraConfiguration>(json, _jsonOptions);
                return configuration == null
                    ? ServiceResult<CameraConfiguration>.Fail($"{fileDescription} поврежден.")
                    : ServiceResult<CameraConfiguration>.Ok(configuration);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации: {Description}.", fileDescription);
                return ServiceResult<CameraConfiguration>.Fail($"{fileDescription} поврежден.");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Ошибка чтения: {Description}.", fileDescription);
                return ServiceResult<CameraConfiguration>.Fail($"Не удалось прочитать: {fileDescription}.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Нет доступа: {Description}.", fileDescription);
                return ServiceResult<CameraConfiguration>.Fail($"Нет доступа к: {fileDescription}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка загрузки: {Description}.", fileDescription);
                return ServiceResult<CameraConfiguration>.Fail($"Не удалось загрузить: {fileDescription}.");
            }
        }
    }
}
