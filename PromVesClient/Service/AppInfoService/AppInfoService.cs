using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PromVesClient.Service.AppInfoService
{
    public class AppInfoService
    {
        private readonly ILogger<AppInfoService> _logger;
        public AppInfoService(ILogger<AppInfoService> logger) 
        {
            _logger = logger;
        }
        //метод предназначен для получении информации о версии программы, пример:
    //    "application": {
    //"name": "RailwayScales", - название программы
    //"version": "1.196.0", - версия программы, где 1 - версия программы изначальная, 196 - регион где будет использоваться эта версия, 0 - версия продукта именно для определенного обьекта 
    //"buildDate": "2026-06-22" - дата создания/сборки проекта
        public string VersionInfo()
        {
            try
            {
                string json = File.ReadAllText("appinfo.json");

                using var doc = JsonDocument.Parse(json);

                string version = doc.RootElement
                .GetProperty("application")
                .GetProperty("version")
                .GetString();
                return ($"Версия: {version}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Некорректный формат appinfo.json");
                return ("Версия: 1.0.0");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("Файл appinfo.json не найден");
                return ("Версия: 1.0.0");
            }
            catch (Exception ex)
            {
                _logger.LogError("Неизвестная ошибка с файлом appinfo.json:", ex.ToString());
                return ("Версия: 1.0.0");
            }
        }
    }
}
