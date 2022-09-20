namespace WineSales.Domain.Exceptions
{
    public class CheckException : Exception
    {
        public CheckException() : base() { }

        public CheckException(string message) : base("CheckException: " + message) { }

        public CheckException(string message, Exception inner) : base(message, inner) { }
    }
}
