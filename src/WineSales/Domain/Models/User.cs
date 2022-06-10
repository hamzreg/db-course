namespace WineSales.Domain.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }
}
