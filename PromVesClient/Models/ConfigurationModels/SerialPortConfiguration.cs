using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models.ConfigurationModels
{
    public class SerialPortConfiguration
    {
        public List<SerialPortSettings> SerialPorts { get; set; } = [];
    }
}
