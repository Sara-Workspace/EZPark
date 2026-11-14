using BLL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Parking.Filters
{
    // השומר המרכזי: תופס כל שגיאה שלא נתפסה, בכל הקונטרולרים
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            // 1. רושמים ביומן את כל הפרטים (רק בשביל המפתחות)
            _logger.LogError(ex, "Unhandled exception at {Path}", context.HttpContext.Request.Path);

            // 2. בוחרים קוד לפי סוג השגיאה
            int status = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,          // לא נמצא
                ConflictException => StatusCodes.Status409Conflict,          // כבר קיים
                BusinessRuleException => StatusCodes.Status400BadRequest,    // נגד הכללים
                ArgumentException => StatusCodes.Status400BadRequest,        // קלט לא תקין (למשל קובץ)
                _ => StatusCodes.Status500InternalServerError                // כל השאר: שגיאה לא צפויה
            };

            // 3. ההודעה למשתמש: בשגיאה לא צפויה לא מגלים פרטים פנימיים
            string message = status == 500
                ? "אירעה שגיאה בלתי צפויה. נסו שוב מאוחר יותר."
                : ex.Message;

            // 4. בונים את התשובה במבנה אחיד
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = status,
                Title = message
            })
            {
                StatusCode = status
            };

            // 5. מסמנים שהשגיאה טופלה
            context.ExceptionHandled = true;
        }
    }
}