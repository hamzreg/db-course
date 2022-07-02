namespace WineSales.Domain.Models
{
    public class SupplierWine
    {
        public int ID { get; set; }
        public int SupplierID { get; set; }
        public int WineID { get; set; }
        public int Percent { get; set; }
    }
}
