using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models
{
    public class Weighing
    {
        public Guid Id { get; set; }
        public string VagonNumber { get; set; }
        //public DateTime DateTime { get; set; }
        //Тара
        public double TareWeight { get; set; }
        //Брутто
        public double GrossWeight { get; set; }
        //Нетто
        public double NetWeight { get; set; }
        //грузоподьемность
        public double LoadCapacity { get; set; }
        //недогруз, перегруз (отклонение нагрузки)
        public double LoadDeviation { get; set; }
        //первая тележка
        public double FirstCart { get; set; }
        //вторая тележка
        public double SecondCart { get; set; }
        //разница тележек
        public double DifferenceCarts { get; set; }
        //вес левого борта
        public double LeftSide { get; set; } 
        //вес правого борта
        public double RightSide { get; set; }
        //разница бортов
        public double DifferenceSides { get; set; }
        // Внешний ключ
        public Guid ReceiptId { get; set; }

        // Навигационное свойство
        public Receipt Receipt { get; set; } = null!;
    }
}
