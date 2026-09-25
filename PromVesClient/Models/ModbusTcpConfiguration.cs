using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class ModbusTcpConfiguration
    {
        public List<ModbusTcpSetting> ModbusTcpSetting { get; set; }
          = new List<ModbusTcpSetting>();
    }
}
