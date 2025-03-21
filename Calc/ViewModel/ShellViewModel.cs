namespace curc_c_.ViewModels
{
    // тут будет бизнес логика
    public class ShellViewModel : BindableBase
    {
        #region property DisplayName

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private string _displayName = "WPF PRISM + DI";

        #endregion
    }
}