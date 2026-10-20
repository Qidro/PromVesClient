using System.IO.Ports;

namespace PromVesClient.Models.ConfigurationModels
{
    /// <summary>
    /// Параметры последовательного порта, используемого устройством PN.
    /// </summary>
    public class PNSerialPortSettings
    {
        /// <summary>
        /// Имя COM-порта, например COM3.
        /// </summary>
        public string PortName { get; set; } = string.Empty;

        /// <summary>
        /// Скорость обмена данными в бодах.
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// Количество бит данных в одном передаваемом символе.
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// Режим контроля чётности.
        /// </summary>
        public Parity Parity { get; set; }

        /// <summary>
        /// Количество стоповых битов.
        /// </summary>
        public StopBits StopBits { get; set; }

        /// <summary>
        /// Режим управления потоком данных.
        /// </summary>
        public Handshake Handshake { get; set; }
    }
}
