using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class SerialPortConfiguration
    {
        public List<SerialPortSettings> SerialPorts { get; set; } = [];
    }
}
