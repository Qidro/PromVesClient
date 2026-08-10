using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class Wagon
    {
        public Guid Id { get; set; }

        // Номер вагона
        public string Number { get; set; } = string.Empty;

        // Тара вагона
        public decimal TareWeight { get; set; }

        // Активен ли вагон
        public bool IsActive { get; set; } = true;
    }
}
