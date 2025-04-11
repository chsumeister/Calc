using System.Collections.Generic;

namespace curc_c_.Model
{
    internal class History
    {
        public IReadOnlyList<string> Entries { get; }

        public History(IEnumerable<string> entries)
        {
            Entries = new List<string>(entries).AsReadOnly();
        }
    }
}