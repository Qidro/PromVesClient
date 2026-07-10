using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class Receipt
    {
        public Guid Id { get; set; }
        //время создания квитанции
        public DateTime DateTime { get; set; }
        //тип взвешивания
        public string TypeWeighng { get; set; }
        //оператор взвегивния
        public string Operator { get; set; }
        // Навигационное свойство
        public ICollection<Weighing> Weighings { get; set; } = new List<Weighing>();
    }
}
