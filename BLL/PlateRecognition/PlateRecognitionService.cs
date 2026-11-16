using BLL.Exceptions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace BLL.PlateRecognition
{
    // קורא הלוחיות: שולח את התמונה ל-Gemini ומבקש ממנו רק את המספר.
    // זה לא צ'אט ואין כאן כלים. שאלה אחת, תשובה אחת
    public class PlateRecognitionService : IPlateRecognitionService
    {
        // ההוראות ל-Gemini. מבקשים תשובה קצרה מאוד, כדי שיהיה קל לבדוק אותה בקוד
        private const string Instructions = """
            בתמונה יש רכב. מצא את לוחית הרישוי שלו.
            ענה רק בספרות של מספר הרישוי, בלי מקפים, בלי רווחים ובלי שום מילה נוספת. לדוגמה: 1234567
            אם אין בתמונה לוחית רישוי, או שאי אפשר לקרוא אותה בבירור, ענה בדיוק: NONE
            אל תנחש ספרות שלא רואים.
            """;

        private readonly IChatClient _chatClient;                     // ה"טלפון" ל-Gemini (נרשם ב-Program.cs, סעיף 6ב)
        private readonly ILogger<PlateRecognitionService> _logger;    // יומן תקלות, רק בשבילנו

        public PlateRecognitionService(IChatClient chatClient, ILogger<PlateRecognitionService> logger)
        {
            _chatClient = chatClient;
            _logger = logger;
        }

        public async Task<string> ReadPlateAsync(byte[] image, string mediaType, CancellationToken ct)
        {
            // הודעה אחת עם שני חלקים: הטקסט (ההוראות) והתמונה (DataContent)
            var message = new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent(Instructions),
                new DataContent(image, mediaType)
            });

            // Temperature = 0: "אל תהיה יצירתי". רוצים תשובה מדויקת, לא המצאות
            var options = new ChatOptions { Temperature = 0 };

            var response = await _chatClient.GetResponseAsync(new List<ChatMessage> { message }, options, ct);

            // משאירים רק ספרות. אם Gemini ענה NONE, יישאר טקסט ריק
            var plate = LicensePlateHelper.Normalize(response.Text);

            if (!LicensePlateHelper.IsValid(plate))
            {
                _logger.LogInformation("Plate not found in image. Gemini answered: {Answer}", response.Text);
                throw new BusinessRuleException("לא הצלחנו לזהות לוחית רישוי בתמונה. נסו לצלם שוב, מקרוב ובאור טוב.");
            }

            return plate;
        }
    }
}