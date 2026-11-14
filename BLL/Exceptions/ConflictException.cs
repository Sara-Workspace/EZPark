namespace BLL.Exceptions
{
    // "כבר קיים": התנגשות עם מה שכבר במערכת (למשל מייל שכבר רשום)
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}