using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Models;

public class ReceiptPrintSettings
{
    public bool VagonNumber { get; set; } = true;
    public bool L1 { get; set; } = true;
    public bool R1 { get; set; } = true;
    public bool L2 { get; set; } = true;
    public bool R2 { get; set; } = true;

    public bool TareWeight { get; set; } = true;
    public bool GrossWeight { get; set; } = true;
    public bool NetWeight { get; set; } = true;

    public bool LoadCapacity { get; set; } = true;
    public bool LoadDeviation { get; set; } = true;

    public bool FirstCart { get; set; } = true;
    public bool SecondCart { get; set; } = true;
    public bool DifferenceCarts { get; set; } = true;

    public bool LeftSide { get; set; } = true;
    public bool RightSide { get; set; } = true;
    public bool DifferenceSides { get; set; } = true;

    public bool TypeWeighing { get; set; } = true;

    public bool Shipper { get; set; } = true;
    public bool Consignee { get; set; } = true;
    public bool Cargo { get; set; } = true;

    public bool InvoiceNumber { get; set; } = true;
    public bool InvoiceDateTime { get; set; } = true;
    public bool InvoiceWeighing { get; set; } = true;
}
