using System;
using System.Collections.Generic;
using System.Text.Json;
using PromVesClient.Models;
using System.IO.Ports;
using System.Linq;
using System.Text.Json.Serialization;

namespace PromVesClient.Service
{

    public class ComPortService
    {
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

        public ComPortService()
        {
            _configurationPath = Path.Combine(
                AppContext.BaseDirectory,
                "Configuration");

            _serverSettingsPath = @"C:\Users\Илья\Desktop\server\settings.json";

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
            if (!File.Exists(_serverSettingsPath))
            {
                return LoadDefaults();
            }

            string json = File.ReadAllText(_serverSettingsPath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return LoadDefaults();
            }

            return JsonSerializer.Deserialize<SerialPortConfiguration>(
     json,
     _jsonOptions)
     ?? LoadDefaults();
        }

        /// <summary>
        /// Загружает настройки по умолчанию.
        /// </summary>
        public SerialPortConfiguration LoadDefaults()
        {
            string json = File.ReadAllText(_defaultSettingsPath);

            return JsonSerializer.Deserialize<SerialPortConfiguration>(
    json,
    _jsonOptions)
    ?? new SerialPortConfiguration();
        }

        /// <summary>
        /// Сохраняет настройки.
        /// </summary>
        public void Save(SerialPortConfiguration configuration)
        {
            string json = JsonSerializer.Serialize(
    configuration,
    _jsonOptions);

            File.WriteAllText(_serverSettingsPath, json);
        }

        /// <summary>
        /// Восстанавливает настройки по умолчанию.
        /// </summary>
        public void RestoreDefaults()
        {
            var configuration = LoadDefaults();

            Save(configuration);
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
        public void Open(SerialPortSettings settings)
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
        }
        // закрытие
        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        //запись
        public void Write(string text)
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("COM-порт не открыт.");
            }

            _serialPort.Write(text);
        }
        //чтение
        public string ReadLine()
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("COM-порт не открыт.");
            }

            return _serialPort.ReadLine();
        }

    }
}    

