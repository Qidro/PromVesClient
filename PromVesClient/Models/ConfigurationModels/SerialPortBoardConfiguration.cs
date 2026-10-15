using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models.ConfigurationModels
{
    public class SerialPortBoardConfiguration
    {
        public List<SerialPortBoard> SerialPortsBoards { get; set; }
           = new List<SerialPortBoard>();
    }
}
