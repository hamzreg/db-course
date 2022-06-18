namespace WineSales.Domain.Exceptions
{
    public class BonusCardException : Exception
    {
        public BonusCardException() : base() { }

        public BonusCardException(string message) : base("BonusCard: " + message) { }

        public BonusCardException(string message, Exception inner) : base(message, inner) { }
    }
}
