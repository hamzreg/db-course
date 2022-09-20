namespace WineSales.Config
{
    public class CheckConfig
    {
        public static List<string> PaymentMethods = new List<string>()
                                            {"card", "cash"};
        public const int MinShiftNumber = 1;
        public const int MaxShiftNumber = 10;
    }
}
