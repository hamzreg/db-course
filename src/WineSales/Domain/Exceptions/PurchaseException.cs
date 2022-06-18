namespace WineSales.Domain.Exceptions
{
    public class PurchaseException : Exception
    {
        public PurchaseException() : base() { }

        public PurchaseException(string message) : base("Purchase: " + message) { }

        public PurchaseException(string message, Exception inner) : base(message, inner) { }
    }
}
