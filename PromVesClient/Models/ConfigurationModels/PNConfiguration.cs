using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models.ConfigurationModels
{
    public class PNConfiguration
    {
        public List<PNSetting> PNSetting { get; set; }
          = new List<PNSetting>();
    }
}
