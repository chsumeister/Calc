namespace curc_c_.Model
{

    // модель для истории
    internal class History
    {
        private List<string> _entries = new List<string>();

        public void AddEntry(string entry)
        {
            _entries.Add(entry);
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public IEnumerable<string> GetEntries()
        {
            return _entries;
        }
    }
}