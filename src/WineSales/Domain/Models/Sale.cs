namespace WineSales.Domain.Models
{
    public class Sale
    {
        public int ID { get; set; }
        public double PurchasePrice { get; set; }
        public double SellingPrice { get; set; }
        public double Margin { get; set; }
        public double Costs { get; set; }
        public double Profit { get; set; }
        public int WineNumber { get; set; }
        public DateOnly Date { get; set; } 
    }
}
