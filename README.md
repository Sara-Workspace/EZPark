# EZPark – צד שרת (.NET 9 Web API)

EZPark הוא אתר שיתופי להשכרת חניות פרטיות: **בעלי חניה** מפרסמים חניה ומגדירים שעות ומחיר, **נהגים** מחפשים חניה קרובה, שומרים אותה, חונים ומשלמים מארנק דיגיטלי, והאתר לוקח עמלה של 10%.

מקביל לשרת קיים פרויקט לקוח ב-React: <https://github.com/Sara-Workspace/EZParkReact>

## יכולות עיקריות
- הרשמה והתחברות (נהג / בעל חניה / מנהל) עם JWT, סיסמאות נשמרות כ-hash
- פרסום חניות עם תמונה, זמינות שבועית ותמחור דינמי
- **חיפוש חניה חכם:** Bounding Box ← מרחק (Haversine) ← ציון מרחק+מחיר ← בחירת K הטובות עם ערימה בינארית (`BinaryHeap`)
- שמירת חניה ל-5 דקות (Hold), "חניתי" ו"סיום חניה" עם חישוב עלות לפי דקות
- ארנק: טעינה דרך PayPal (Sandbox) עם אימות בשרת, חלוקת תשלום 90% לבעלים / 10% לאתר
- בדיקת חניה תפוסה עם זיהוי לוחית רישוי (Gemini) וקנסות
- **Agent AI** (Gemini) עם כלים (Tools) לחיפוש חניה, והזרמת התשובה ב-SSE
- API חיצוני: OpenStreetMap (Geocoding), PayPal, Gemini

## ארכיטקטורה
```
Client → Controller → Service → Repository → DbContext → SQL Server
```

| פרויקט | תפקיד |
|---|---|
| `DTO` | אובייקטי העברה ל/מהלקוח (בלי חשיפת Entities) |
| `DAL` | Entities, `ParkingDbContext`, Repositories, Migrations |
| `BLL` | לוגיקה עסקית (Services), AutoMapper, Exceptions, Search, Agent, Payments |
| `EZPark` | שכבת Web: Controllers, `Program.cs`, `GlobalExceptionFilter` |

## דרישות
- .NET 9 SDK
- SQL Server (או LocalDB)
- כלי EF: `dotnet tool install --global dotnet-ef`

## משתני סביבה
**הסודות לא נמצאים בקוד או ב-GitHub.** יש להגדיר אותם בעצמך:

| משתנה | חובה | הסבר |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | כן | מחרוזת חיבור ל-SQL Server |
| `Jwt__Key` | כן | מפתח חתימה ל-JWT, לפחות 32 תווים |
| `Gemini__ApiKey` | כן | מפתח Gemini. **בלעדיו השרת לא עולה** |
| `PayPal__ClientId` | לטעינת ארנק | Client ID של PayPal Sandbox |
| `PayPal__Secret` | לטעינת ארנק | Secret של PayPal Sandbox |
| `ImagesFolder` | לא | תיקיית תמונות (ברירת מחדל `../Images`, נוצרת אוטומטית) |

הגדרה עם User Secrets (מומלץ לפיתוח):
```bash
cd EZPark
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string>"
dotnet user-secrets set "Jwt:Key" "<at least 32 characters>"
dotnet user-secrets set "Gemini:ApiKey" "<your key>"
dotnet user-secrets set "PayPal:ClientId" "<sandbox client id>"
dotnet user-secrets set "PayPal:Secret" "<sandbox secret>"
```

או כמשתני סביבה (PowerShell):
```powershell
$env:ConnectionStrings__DefaultConnection = "<your connection string>"
$env:Jwt__Key = "<at least 32 characters>"
$env:Gemini__ApiKey = "<your key>"
```

## הרצה
```bash
# 1. יצירת מסד הנתונים (מהתיקייה הראשית של ה-Solution)
dotnet ef database update --project DAL --startup-project EZPark

# 2. הרצת השרת
dotnet run --project EZPark --launch-profile https
```
- השרת עולה ב-`https://localhost:7036` (ו-`http://localhost:5056`).
- ב-Development נפתח **Swagger** עם כפתור Authorize להזנת הטוקן.
- CORS מאפשר את הלקוח ב-`http://localhost:5173` וב-`ezpark-app.tryasp.net` (פריסה). `AllowedHosts` מוגבל ל-`ezpark.tryasp.net;localhost`.

## משתמש מנהל
הרשמה רגילה יוצרת נהג או בעל חניה. כדי ליצור מנהל, מסמנים משתמש קיים ידנית במסד: `UPDATE Users SET IsAdmin = 1 WHERE Email = '...'`.

## נקודות קצה עיקריות
| נושא | נתיבים |
|---|---|
| התחברות | `POST /api/auth/login` |
| הרשמה | `POST /api/drivers`, `POST /api/owners` |
| חניות | `GET/POST/PUT/DELETE /api/parkingspots`, `GET /api/parkingspots/search`, `POST/DELETE /api/parkingspots/{id}/hold` |
| הזמנות | `POST /api/reservations/start/{spotId}`, `POST /api/reservations/{id}/complete`, `GET /api/reservations/my` |
| ארנק | `GET /api/wallet/me`, `POST /api/wallet/topup/create`, `POST /api/wallet/topup/capture` |
| ביקורות | `POST /api/reviews`, `GET /api/reviews/spot/{id}` |
| בדיקות חניה | `/api/spotchecks/...` |
| צ'אט AI | `POST /api/chat/stream` |

## אבטחה
- סיסמאות: `PasswordHasher` (hash עם salt)
- JWT עם Issuer, Audience ותוקף; מזהה המשתמש נלקח מהטוקן ולא מהלקוח
- הרשאות לפי תפקיד (`[Authorize(Roles=...)]`) ובדיקות בעלות בכל Service
- שגיאות מטופלות ב-`GlobalExceptionFilter` ולא חושפות פרטים פנימיים
- אימות תשלום PayPal מתבצע בשרת (סכום ומטבע), ומניעת טעינה כפולה

## פריסה
קובץ `.github/workflows/main.yml` בונה ומפרסם לשרת האירוח (tryasp.net). פרטי ההתחברות לשרת האירוח ומשתני הסביבה של האפליקציה מוגדרים בהגדרות האירוח וב-GitHub Secrets, **לא בקוד**.