using DAL;

namespace DAL.Repositories
{
    // גישה לנתוני הארנקים: היתרה (בטבלת Users) ודף החשבון (WalletTransactions)
    public interface IWalletRepository
    {
        Users? GetUserByEmail(string email);
        List<WalletTransaction> GetTransactionsByUserId(int userId, int take);
        WalletTransaction? GetByPayPalOrderId(string orderId);
        void AddTransaction(WalletTransaction transaction);            // שורה אחת (+ שמירה)
        void AddTransactions(List<WalletTransaction> transactions);    // כמה שורות שחייבות להישמר ביחד (+ שמירה אחת)
        void UpdateTransaction(WalletTransaction transaction);
        decimal GetPlatformTotal(string type);   // כמה האתר הרוויח מסוג מסוים (עמלות / קנסות)
        int CountPlatform(string type);          // כמה פעמים
    }
}
