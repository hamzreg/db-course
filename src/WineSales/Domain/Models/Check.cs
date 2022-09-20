namespace WineSales.Domain.Models
{
    public class Check
    {
        public int ID { get; set; }
        public string PaymentMethod { get; set; }
        public int ShiftNumber { get; set; }
        public int SaleID { get; set; }
    }
}
