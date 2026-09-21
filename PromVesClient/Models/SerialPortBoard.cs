using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace PromVesClient.Models
{
    public class SerialPortBoard
    {
        public int Id { get; set; }

        public string PortName { get; set; } = string.Empty;

        public int BaudRate { get; set; }

        public int DataBits { get; set; }
        public Parity Parity { get; set; }

        public StopBits StopBits { get; set; }

        public Handshake Handshake { get; set; }
    }
}
