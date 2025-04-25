using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Calc.Model;
using System.Windows.Media;

namespace Calc.ViewModels
{
    public class HistoryViewModel : BindableBase
    {
        private History _history;
        private string _selectedHistoryEntry;
        private readonly Action _closeHistoryAction;

        /// <summary>
        /// Цвет текста записей в истории.
        /// </summary>
        private Brush _historyTextColor;
        public Brush HistoryTextColor
        {
            get => _historyTextColor;
            set => SetProperty(ref _historyTextColor, value);
        }

        public HistoryViewModel(Action closeHistoryAction = null)
        {
            _history = new History(Array.Empty<string>());
            _closeHistoryAction = closeHistoryAction;

            ClearHistoryCommand = new DelegateCommand(ClearHistory);
            CloseHistoryCommand = new DelegateCommand(() => _closeHistoryAction?.Invoke());

            DeleteEntryCommand = new DelegateCommand<string>(DeleteEntry);
        }

        /// <summary>
        /// Список всех записей в истории.
        /// </summary>
        public IReadOnlyList<string> Entries => _history.Entries;

        public ICommand ClearHistoryCommand { get; }
        public ICommand CloseHistoryCommand { get; set; }
        public ICommand DeleteEntryCommand { get; }

        /// <summary>
        /// Выбранная запись истории.
        /// </summary>
        public string SelectedHistoryEntry
        {
            get => _selectedHistoryEntry;
            set
            {
                if (SetProperty(ref _selectedHistoryEntry, value) && !string.IsNullOrEmpty(value))
                {
                    OnHistoryEntrySelected(value);
                }
            }
        }

        /// <summary>
        /// Событие, вызываемое при выборе записи из истории.
        /// </summary>
        public event Action<string> HistoryEntrySelected;

        /// <summary>
        /// Добавляет новую запись в историю.
        /// </summary>
        /// <param name="entry">Запись для добавления.</param>
        public void AddEntry(string entry)
        {
            var newEntries = new List<string>(_history.Entries) { entry };
            _history = new History(newEntries);
            RaisePropertyChanged(nameof(Entries));
        }

        /// <summary>
        /// Удаляет указанную запись из истории.
        /// </summary>
        /// <param name="entry">Запись для удаления.</param>
        private void DeleteEntry(string entry)
        {
            var newEntries = new List<string>(_history.Entries);
            newEntries.Remove(entry);
            _history = new History(newEntries);
            RaisePropertyChanged(nameof(Entries));
        }

        /// <summary>
        /// Очищает всю историю.
        /// </summary>
        private void ClearHistory()
        {
            _history = new History(Array.Empty<string>());
            RaisePropertyChanged(nameof(Entries));
        }

        /// <summary>
        /// Вызывает событие при выборе записи истории.
        /// </summary>
        /// <param name="entry">Выбранная запись.</param>
        private void OnHistoryEntrySelected(string entry)
        {
            HistoryEntrySelected?.Invoke(entry);
        }
    }
}
