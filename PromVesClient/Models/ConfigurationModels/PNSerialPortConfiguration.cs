using System.Collections.Generic;

namespace PromVesClient.Models.ConfigurationModels
{
    /// <summary>
    /// Корневой объект конфигурационного файла ConfigPN.json.
    /// </summary>
    public class PNSerialPortConfiguration
    {
        /// <summary>
        /// Список настроек PN-портов. Сейчас используется один порт,
        /// но коллекция сохраняет структуру, аналогичную ConfigPort.json.
        /// </summary>
        public List<PNSerialPortSettings> SerialPorts { get; set; } = new();
    }
}
