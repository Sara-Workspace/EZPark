namespace BLL.Exceptions
{
    // "לא מצאתי": המבוקש לא קיים (חניה, הזמנה, משתמש וכו')
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}