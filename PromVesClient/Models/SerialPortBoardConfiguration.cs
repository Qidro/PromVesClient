using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class SerialPortBoardConfiguration
    {
        public List<SerialPortBoard> SerialPortsBoards { get; set; }
           = new List<SerialPortBoard>();
    }
}
