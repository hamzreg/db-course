namespace WineSales.Domain.Models
{
    public class Purchase
    {
        public int ID { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }
        public int CustomerID { get; set; }
    }
}
