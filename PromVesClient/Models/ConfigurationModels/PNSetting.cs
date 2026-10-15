using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models.ConfigurationModels
{
    public class PNSetting
    {
        //номер канала
        public int ChannelNumber { get; set; }
        //код нуля
        public decimal ZeroCode { get; set; }
        //код эталона
        public decimal ReferenceCode { get; set; }
        //эталонный вес
        public decimal ReferenceWeight { get; set; }
        //коэффицент эталонного веса
        public decimal WeightCoefficient { get; set; }
    }
}
