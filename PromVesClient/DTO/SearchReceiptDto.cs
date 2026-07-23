using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.DTO
{
    //DTO для создания фильтра к 
    public class SearchReceiptDto
    {
        //период с
        public DateTime periodStart { get; set; }
        //период по
        public DateTime periodEnd { get; set; }
        //логин оператора
        public string? operatorName { get; set; }
        //номер вагона
        public string? vagonNumber { get; set; }
    }
}
