using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.DTO
{
    public class ReceiptDto
    {
        public Guid Id { get; set; }
        //время создания квитанции
        public DateTime DateTime { get; set; }
        //тип взвешивания
        public string? TypeWeighng { get; set; }
        //оператор взвешивания
        public string? Operator { get; set; }
        // Навигационное свойство
    }
}
