using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using curc_c_.Model;

namespace curc_c_.ViewModels
{
    public class HistoryViewModel : BindableBase
    {
        private History _history;
        private string _selectedHistoryEntry;

        public HistoryViewModel()
        {
            _history = new History(Array.Empty<string>());
            ClearHistoryCommand = new DelegateCommand(ClearHistory);
        }

        public IReadOnlyList<string> Entries => _history.Entries;
        public ICommand ClearHistoryCommand { get; }
        public ICommand CloseCommand { get; }

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

        public event Action<string> HistoryEntrySelected;


        /// <summary>
        /// Добавляет новую запись в историю вычислений
        /// </summary>
        /// <param name="entry">Текст записи для добавления</param>
        public void AddEntry(string entry)
        {
            var newEntries = new List<string>(_history.Entries) { entry };
            _history = new History(newEntries);
            RaisePropertyChanged(nameof(Entries));
        }


        /// <summary>
        /// Очищает историю вычислений
        /// </summary>
        private void ClearHistory()
        {
            _history = new History(Array.Empty<string>());
            RaisePropertyChanged(nameof(Entries));
        }


        /// <summary>
        /// Вызывается при выборе записи из истории
        /// </summary>
        /// <param name="entry">Выбранная запись</param>
        private void OnHistoryEntrySelected(string entry)
        {
            HistoryEntrySelected?.Invoke(entry);
        }
    }
}