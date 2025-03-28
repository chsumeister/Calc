using curc_c_.Model;

namespace curc_c_.ViewModels
{
    public class HistoryViewModel : BindableBase
    {
        private readonly History _history = new History();

        public IEnumerable<string> History => _history.GetEntries();

        // добавляет в историю 
        public void AddEntry(string entry)
        {
            _history.AddEntry(entry);
        }
        // очистка истории
        public void ClearHistory()
        {
            _history.Clear();
        }

        private string _selectedHistoryEntry;

        // хранит запись из истории 
        public string SelectedHistoryEntry
        {
            get => _selectedHistoryEntry;
            set
            {
                SetProperty(ref _selectedHistoryEntry, value);
                if (!string.IsNullOrEmpty(value))
                {
                    OnHistoryEntrySelected(value);
                }
            }
        }
        //событие для того чтобы подписать в шеллвьюмодел (в конструкторе)
        public event Action<string> HistoryEntrySelected;


        // это для передачи, чтобы не было неприятных ситуаций, если подписчика не будет (в конструкторе шеллвьюмодел)
        private void OnHistoryEntrySelected(string entry)
        {
            HistoryEntrySelected?.Invoke(entry);
        }
    }
}