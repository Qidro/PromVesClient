using System.IO.Ports;

namespace PromVesClient.Models.ConfigurationModels
{
    // Содержит все параметры последовательного порта, используемого устройством PN.
    public class PNSerialPortSettings
    {
        // Имя COM-порта, например COM3.
        public string PortName { get; set; } = string.Empty;

        // Скорость обмена данными в бодах.
        public int BaudRate { get; set; }

        // Количество бит данных в одном передаваемом символе.
        public int DataBits { get; set; }

        // Режим контроля чётности.
        public Parity Parity { get; set; }

        // Количество стоповых битов.
        public StopBits StopBits { get; set; }

        // Режим управления потоком данных.
        public Handshake Handshake { get; set; }
    }
}
