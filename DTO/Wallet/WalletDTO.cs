namespace DTO.Wallet
{
    // מה רואים במסך "הארנק שלי"
    public class WalletDTO
    {
        public decimal Balance { get; set; }                                   // היתרה בשקלים
        public List<WalletTransactionDTO> Transactions { get; set; } = new();  // התנועות האחרונות
    }

    // שורה אחת בדף החשבון
    public class WalletTransactionDTO
    {
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }          // חיובי = נכנס, שלילי = יצא
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
