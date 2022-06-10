namespace WineSales.Config
{
    public class Constants
    {
        public const int SupplierRole = 0;
        public const int CustomerRole = 1;
        public const int AdminRole = 2;

        // Wine
        public static List<string> Colors = new List<string>() 
                                            { "red", "white", "rose"};
        public static List<string> Sugar = new List<string>()
                                            { "dry", "semi-dry", "semi-sweet", "sweet"};

        public const double MinVolume = 0.1875;
        public const double MaxVolume = 30;

        public const double MinAlcohol = 7.5;
        public const double MaxAlcohol = 22;

        public const int MinAging = 2;
        public const int MaxAging = 10;

        // BonusCard
        public const int MaxPhoneLen = 11;

        public const double MinBonusCard = 0;

        public const int ErrorValue = -1;
    }

    public class ReturnCodes
    {
        public enum Codes
        {
            OK,
            InvalidInput,
            NotExists,
            Empty
        }
    }
}
