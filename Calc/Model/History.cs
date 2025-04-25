using System.Collections.Generic;

namespace Calc.Model
{
    public class History
    {
        public IReadOnlyList<string> Entries { get; }

        public History(IEnumerable<string> entries)
        {
            Entries = new List<string>(entries).AsReadOnly();
        }
    }
}