namespace BLL.Services
{
    // סוגי התנועות בארנק (כדי לא לכתוב את אותו טקסט בהרבה מקומות ולטעות באות)
    public static class WalletTransactionTypes
    {
        public const string TopUp = "TopUp";             // נהג טען כסף ב-PayPal (+)
        public const string Parking = "Parking";         // נהג שילם על חניה (-)
        public const string Earning = "Earning";         // בעל חניה קיבל 90% (+)
        public const string PlatformFee = "PlatformFee"; // האתר קיבל 10% (+, בלי משתמש)
        public const string Fine = "Fine";               // נהג קיבל קנס (-), והאתר קיבל אותו (+)
        public const string Withdrawal = "Withdrawal";   // בעל חניה משך כסף (-)
    }
}
