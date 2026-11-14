namespace BLL.Exceptions
{
    // "נגד הכללים": הפרה של כלל באתר (למשל הזמנה לזמן שעבר)
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}