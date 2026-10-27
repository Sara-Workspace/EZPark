using System.Security.Claims;

namespace Parking.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // מתודה שמחלצת את מזהה המשתמש מתוך ה-JWT בכל קונטרולר
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(value, out var id))
                throw new InvalidOperationException("User ID claim is missing.");

            return id;
        }
    }
}