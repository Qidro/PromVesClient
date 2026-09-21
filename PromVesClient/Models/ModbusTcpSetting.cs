using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class ModbusTcpSetting
    {
        public int Id { get; set; }

        public string NportIp { get; set; } = string.Empty;

        public int NportPort { get; set; }

        public int SlaveId { get; set; }
    }
}
