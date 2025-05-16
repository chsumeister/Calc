using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Calc.Model
{
    public class History
    {
        [JsonInclude]
        public List<HistoryEntry> Entries { get; private set; }

        [JsonConstructor]
        public History(List<HistoryEntry> entries)
        {
            Entries = entries ?? new List<HistoryEntry>();
        }

        public History(IEnumerable<HistoryEntry> entries)
        {
            Entries = new List<HistoryEntry>(entries);
        }
    }
}
