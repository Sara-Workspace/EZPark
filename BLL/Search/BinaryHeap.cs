namespace BLL.Search
{
    // ערימה בינארית (Binary Heap) שנכתבה ביד.
    // השורש הוא תמיד האיבר ש"יוצא ראשון" לפי ההשוואה שנתנו לה.
    public class BinaryHeap<T>
    {
        private readonly List<T> _items = new();
        private readonly Comparison<T> _comesFirst; // מספר שלילי = a יוצא לפני b

        public BinaryHeap(Comparison<T> comesFirst)
        {
            _comesFirst = comesFirst;
        }

        public int Count => _items.Count;

        // מי נמצא בשורש (בלי להוציא אותו)
        public T Peek()
        {
            if (_items.Count == 0) throw new InvalidOperationException("הערימה ריקה");
            return _items[0];
        }

        // הוספה: שמים בסוף ו"מעלים" למעלה כל עוד הוא צריך לצאת לפני ההורה. O(log n)
        public void Push(T item)
        {
            _items.Add(item);
            SiftUp(_items.Count - 1);
        }

        // הוצאת השורש: מחליפים אותו באיבר האחרון ו"מורידים" למטה למקום הנכון. O(log n)
        public T Pop()
        {
            if (_items.Count == 0) throw new InvalidOperationException("הערימה ריקה");

            T root = _items[0];
            T last = _items[^1];
            _items.RemoveAt(_items.Count - 1);

            if (_items.Count > 0)
            {
                _items[0] = last;
                SiftDown(0);
            }
            return root;
        }

        // החלפת השורש באיבר חדש (זול יותר מ-Pop ואחריו Push). O(log n)
        public void ReplaceRoot(T item)
        {
            if (_items.Count == 0) throw new InvalidOperationException("הערימה ריקה");
            _items[0] = item;
            SiftDown(0);
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_comesFirst(_items[index], _items[parent]) >= 0) break;
                Swap(index, parent);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            int count = _items.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = left + 1;
                int first = index;

                if (left < count && _comesFirst(_items[left], _items[first]) < 0) first = left;
                if (right < count && _comesFirst(_items[right], _items[first]) < 0) first = right;

                if (first == index) break;
                Swap(index, first);
                index = first;
            }
        }

        private void Swap(int a, int b)
        {
            (_items[a], _items[b]) = (_items[b], _items[a]);
        }
    }

    public static class HeapSelection
    {
        // בחירת K הטובים ביותר מתוך n איברים, בלי למיין את כולם. זמן: O(n log K)
        // הרעיון: שומרים ערימה בגודל K שבראשה הגרוע מבין הטובים שנמצאו עד עכשיו.
        // איבר חדש נכנס רק אם הוא טוב יותר מהגרוע הזה, ואז הוא מחליף אותו.
        public static List<T> SelectTopK<T>(IEnumerable<T> items, int k, Comparison<T> bestFirst)
        {
            if (k <= 0) return new List<T>();

            var worstOnTop = new BinaryHeap<T>((a, b) => bestFirst(b, a));

            foreach (var item in items)
            {
                if (worstOnTop.Count < k)
                {
                    worstOnTop.Push(item);
                }
                else if (bestFirst(item, worstOnTop.Peek()) < 0)
                {
                    worstOnTop.ReplaceRoot(item);
                }
            }

            // מוציאים מהגרוע לטוב, ולכן ממלאים את התוצאה מהסוף להתחלה
            var result = new T[worstOnTop.Count];
            for (int i = result.Length - 1; i >= 0; i--)
            {
                result[i] = worstOnTop.Pop();
            }
            return result.ToList();
        }
    }
}