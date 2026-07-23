using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.DTO
{
    public class WeighingDto
    {
        
        public double Platform1Left { get; set; }

        public double Platform1Right { get; set; } 
        public double Platform2Left { get; set; }
        public double Platform2Right { get; set; }
        public string VagonNumber { get; set; }
        public double TareWeight { get; set; }
        public double GrossWeight { get; set; }

        public string TypeWeighing { get; set; }
        public Guid IdReceipt { get; set; }
    }
}
