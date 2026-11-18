namespace BLL.Search
{
    // שיטת המיון: חכם (מרחק + מחיר ביחד), לפי מרחק בלבד, או לפי מחיר בלבד
    public enum SpotSortMode
    {
        Smart,
        Distance,
        Price
    }

    // חניה מועמדת: רק הנתונים שהאלגוריתם צריך
    public record SpotCandidate(int SpotId, double Latitude, double Longitude, decimal PricePerHour);

    // תוצאה מדורגת. ציון נמוך יותר = חניה טובה יותר
    public record RankedSpot(int SpotId, double DistanceKm, decimal PricePerHour, double Score);

    // האלגוריתם עצמו: פונקציות טהורות, בלי מסד נתונים, ולכן קל לבדוק אותן
    public static class SpotSearchEngine
    {
        private const double EarthRadiusKm = 6371.0;
        private const double KmPerDegreeLat = 111.32;

        // "ריבוע" סביב היעד. משמש לסינון זול במסד הנתונים לפני כל חישוב יקר
        public readonly record struct BoundingBox(double MinLat, double MaxLat, double MinLng, double MaxLng);

        public static BoundingBox GetBoundingBox(double lat, double lng, double radiusKm)
        {
            double deltaLat = radiusKm / KmPerDegreeLat;

            // ככל שמתרחקים מקו המשווה, מעלת אורך קצרה יותר
            double cosLat = Math.Max(Math.Cos(ToRadians(lat)), 0.01);
            double deltaLng = radiusKm / (KmPerDegreeLat * cosLat);

            return new BoundingBox(lat - deltaLat, lat + deltaLat, lng - deltaLng, lng + deltaLng);
        }

        // מרחק אווירי בק"מ (נוסחת Haversine)
        public static double DistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLng = ToRadians(lng2 - lng1);

            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            return 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(h));
        }

        // הדירוג: 1) מרחק מדויק ו"עיגול" ברדיוס, 2) ציון, 3) מיון, 4) חיתוך לכמות מבוקשת
        public static List<RankedSpot> Rank(
            IEnumerable<SpotCandidate> candidates,
            double lat,
            double lng,
            double radiusKm,
            SpotSortMode mode,
            int limit,
            double distanceWeight = 0.6)
        {
            if (radiusKm <= 0 || limit <= 0) return new List<RankedSpot>();

            var inRange = candidates
                .Select(c => (Spot: c, Distance: DistanceKm(lat, lng, c.Latitude, c.Longitude)))
                .Where(x => x.Distance <= radiusKm)
                .ToList();

            if (inRange.Count == 0) return new List<RankedSpot>();

            // המחיר מושווה ליקר ביותר שנמצא באזור (למשל 10 מול 40 = רבע)
            double maxPrice = (double)inRange.Max(x => x.Spot.PricePerHour);

            var scored = inRange.Select(x =>
            {
                double score = mode switch
                {
                    SpotSortMode.Distance => x.Distance,
                    SpotSortMode.Price => (double)x.Spot.PricePerHour,
                    // חכם: שני המדדים מוכנסים לסקאלה משותפת 0..1 ומוכפלים במשקל.
                    // מרחק: איזה חלק מהרדיוס שבחר המשתמש. מחיר: איזה חלק מהחניה היקרה באזור
                    _ => distanceWeight * (x.Distance / radiusKm) +
                         (1 - distanceWeight) * (maxPrice <= 0 ? 0 : (double)x.Spot.PricePerHour / maxPrice)
                };
                return new ScoredSpot(x.Spot, x.Distance, score);
            });

            // במקום למיין את כל החניות, בוחרים רק את ה-limit הטובות ביותר בעזרת ערימה
            var top = HeapSelection.SelectTopK(scored, limit, mode == SpotSortMode.Price ? ComparePrice : CompareScore);

            return top
                .Select(x => new RankedSpot(x.Spot.SpotId, x.Distance, x.Spot.PricePerHour, x.Score))
                .ToList();
        }

        private record ScoredSpot(SpotCandidate Spot, double Distance, double Score);

        // מי טוב יותר? ציון נמוך קודם. בשוויון: מחיר זול יותר, ואז מזהה (כדי שהסדר יהיה קבוע)
        private static int CompareScore(ScoredSpot a, ScoredSpot b)
        {
            int c = a.Score.CompareTo(b.Score);
            if (c != 0) return c;
            c = a.Spot.PricePerHour.CompareTo(b.Spot.PricePerHour);
            if (c != 0) return c;
            return a.Spot.SpotId.CompareTo(b.Spot.SpotId);
        }

        // לפי מחיר: מחיר נמוך קודם. בשוויון: הקרובה יותר, ואז מזהה
        private static int ComparePrice(ScoredSpot a, ScoredSpot b)
        {
            int c = a.Score.CompareTo(b.Score);
            if (c != 0) return c;
            c = a.Distance.CompareTo(b.Distance);
            if (c != 0) return c;
            return a.Spot.SpotId.CompareTo(b.Spot.SpotId);
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}