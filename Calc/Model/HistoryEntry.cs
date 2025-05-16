using System;

namespace Calc.Model
{
    public class HistoryEntry
    {
        public string Expression { get; set; }
        public DateTime Timestamp { get; set; }

        public HistoryEntry(string expression)
        {
            Expression = expression;
            Timestamp = DateTime.Now;
        }

        public HistoryEntry() { }

        public override string ToString() => Expression;
    }
}