using Calc.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Media;

namespace Calc.ViewModels
{
    public class HistoryViewModel : BindableBase
    {
        private History _history;
        private HistoryEntry? _selectedHistoryEntry;
        private readonly Action? _closeHistoryAction;
        private static readonly string HistoryFilePath = Path.Combine(AppContext.BaseDirectory, "history.json");

        private Brush? _historyTextColor;

        /// <summary>
        /// Цвет текста для отображения истории.
        /// </summary>
        public Brush HistoryTextColor
        {
            get => _historyTextColor!;
            set => SetProperty(ref _historyTextColor, value);
        }

        /// <summary>
        /// Конструктор ViewModel истории.
        /// </summary>
        public HistoryViewModel(Action? closeHistoryAction = null)
        {
            _history = new History(Array.Empty<HistoryEntry>());
            _closeHistoryAction = closeHistoryAction;

            ClearHistoryCommand = new DelegateCommand(ClearHistory);
            CloseHistoryCommand = new DelegateCommand(() => _closeHistoryAction?.Invoke());
            DeleteEntryCommand = new DelegateCommand<HistoryEntry>(DeleteEntry);

            LoadHistory();
        }

        /// <summary>
        /// Список всех записей истории.
        /// </summary>
        public IReadOnlyList<HistoryEntry> Entries => _history.Entries;
        public ICommand ClearHistoryCommand { get; }
        public ICommand CloseHistoryCommand { get; set; }
        public ICommand DeleteEntryCommand { get; }

        /// <summary>
        /// Выбранная пользователем запись из истории.
        /// </summary>
        public HistoryEntry? SelectedHistoryEntry
        {
            get => _selectedHistoryEntry;
            set
            {
                if (SetProperty(ref _selectedHistoryEntry, value) && value != null)
                {
                    OnHistoryEntrySelected(value);
                }
            }
        }

        /// <summary>
        /// Событие, возникающее при выборе записи из истории.
        /// </summary>
        public event Action<string>? HistoryEntrySelected;

        /// <summary>
        /// Добавляет новую запись в историю.
        /// </summary>
        /// <param name="expression">Текстовое выражение для добавления.</param>
        public void AddEntry(string expression)
        {
            var newEntries = new List<HistoryEntry>(_history.Entries)
            {
                new HistoryEntry(expression)
            };
            _history = new History(newEntries);
            RaisePropertyChanged(nameof(Entries));
            SaveHistory();
        }

        /// <summary>
        /// Удаляет указанную запись из истории.
        /// </summary>
        /// <param name="entry">Запись, которую нужно удалить.</param>
        private void DeleteEntry(HistoryEntry entry)
        {
            var newEntries = new List<HistoryEntry>(_history.Entries);
            newEntries.Remove(entry);
            _history = new History(newEntries);
            RaisePropertyChanged(nameof(Entries));
            SaveHistory();
        }

        /// <summary>
        /// Очищает всю историю.
        /// </summary>
        private void ClearHistory()
        {
            _history = new History(Array.Empty<HistoryEntry>());
            RaisePropertyChanged(nameof(Entries));
            SaveHistory();
        }

        /// <summary>
        /// Вызывает событие выбора записи из истории.
        /// </summary>
        /// <param name="entry">Выбранная запись.</param>
        private void OnHistoryEntrySelected(HistoryEntry entry)
        {
            HistoryEntrySelected?.Invoke(entry.Expression);
        }

        /// <summary>
        /// Загружает историю из JSON-файла.
        /// </summary>
        public void LoadHistory()
        {
            if (File.Exists(HistoryFilePath))
            {
                try
                {
                    var json = File.ReadAllText(HistoryFilePath);
                    var loadedHistory = JsonSerializer.Deserialize<History>(json);
                    _history = loadedHistory ?? new History(Array.Empty<HistoryEntry>());
                    RaisePropertyChanged(nameof(Entries));
                }
                catch
                {
                    _history = new History(Array.Empty<HistoryEntry>());
                }
            }
        }

        /// <summary>
        /// Сохраняет текущую историю в JSON-файл.
        /// </summary>
        public void SaveHistory()
        {
            var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(HistoryFilePath, json);
        }
    }
}
