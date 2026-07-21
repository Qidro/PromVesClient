using System;
using System.Collections.Generic;
using System.Text.Json;
using PromVesClient.Models;

namespace PromVesClient.Service
{

    public class ComPortSettingsService
    {
        private readonly string _configurationPath;
        private readonly string _settingsPath;
        private readonly string _defaultSettingsPath;

        public ComPortSettingsService()
        {
            _configurationPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration");

            _settingsPath = Path.Combine(
                _configurationPath,
                "settings.json");

            _defaultSettingsPath = Path.Combine(
                _configurationPath,
                "defaultSettings.json");
        }


        /// <summary>
        /// Загружает текущие настройки.
        /// Если файла нет — создает его из defaultSettings.json.
        /// </summary>
        public SerialPortConfiguration Load()
        {
            EnsureConfigurationExists();

            string json = File.ReadAllText(_settingsPath);

            return JsonSerializer.Deserialize<SerialPortConfiguration>(json)
                   ?? new SerialPortConfiguration();
        }

        /// <summary>
        /// Загружает настройки по умолчанию.
        /// </summary>
        public SerialPortConfiguration LoadDefaults()
        {
            string json = File.ReadAllText(_defaultSettingsPath);

            return JsonSerializer.Deserialize<SerialPortConfiguration>(json)
                   ?? new SerialPortConfiguration();
        }

        /// <summary>
        /// Сохраняет настройки.
        /// </summary>
        public void Save(SerialPortConfiguration configuration)
        {
            EnsureConfigurationDirectory();

            string json = JsonSerializer.Serialize(
                configuration,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_settingsPath, json);
        }

        /// <summary>
        /// Восстанавливает настройки по умолчанию.
        /// </summary>
        public void RestoreDefaults()
        {
            EnsureConfigurationDirectory();

            if (!File.Exists(_defaultSettingsPath))
            {
                throw new FileNotFoundException(
                    "Не найден файл defaultSettings.json",
                    _defaultSettingsPath);
            }

            File.Copy(
                _defaultSettingsPath,
                _settingsPath,
                true);
        }

        private void EnsureConfigurationExists()
        {
            EnsureConfigurationDirectory();

            if (!File.Exists(_settingsPath))
            {
                RestoreDefaults();
            }
        }

        private void EnsureConfigurationDirectory()
        {
            if (!Directory.Exists(_configurationPath))
            {
                Directory.CreateDirectory(_configurationPath);
            }
        }
    }
}    

