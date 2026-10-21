using System.Collections.Generic;

namespace PromVesClient.Models.ConfigurationModels
{
    // Корневой объект конфигурационного файла ConfigPN.json.
    public class PNSerialPortConfiguration
    {
        // Список настроек PN-портов. Сейчас используется один порт,
        // но коллекция сохраняет структуру, аналогичную ConfigPort.json.
        public List<PNSerialPortSettings> SerialPorts { get; set; } = new();
    }
}
