using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.DTO
{
    public  class CardsDto 
    {
        public Guid Id { get; set; }
        public string VagonNumber { get; set; }
        //public DateTime DateTime { get; set; }
        public decimal L1 { get; set; }
        //правая сторона первой тележки
        public decimal R1 { get; set; }
        //левая сторона второй тележки
        public decimal L2 { get; set; }
        //правая сторона второй тележки
        public decimal R2 { get; set; }
        //Тара
        public decimal TareWeight { get; set; }
        //Брутто
        public decimal GrossWeight { get; set; }
        //Нетто
        public decimal NetWeight { get; set; }
        //грузоподьемность
        public decimal LoadCapacity { get; set; }
        //недогруз, перегруз (отклонение нагрузки)
        public decimal LoadDeviation { get; set; }
        //первая тележка
        public decimal FirstCart { get; set; }
        //вторая тележка
        public decimal SecondCart { get; set; }
        //разница тележек
        public decimal DifferenceCarts { get; set; }
        //вес левого борта
        public decimal LeftSide { get; set; }
        //вес правого борта
        public decimal RightSide { get; set; }
        //разница бортов
        public decimal DifferenceSides { get; set; }
        //тип взвешивания
        public string TypeWeighing { get; set; }
        //грузоотправитель
        public string? Shipper { get; set; }
        //грузополучатель
        public string? Consignee { get; set; }
        //груз
        public string? Cargo { get; set; }
        //номер накладной
        public string? InvoiceNumber { get; set; }
        //номер накладной
        public DateTime? InvoiceDateTime { get; set; }
        //вес по накладной
        public decimal? InvoiceWeighing { get; set; }
        // Внешний ключ
        public Guid ReceiptId { get; set; }

    }
}
