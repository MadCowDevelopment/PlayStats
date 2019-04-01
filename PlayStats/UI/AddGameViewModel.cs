using System.Globalization;
using System.Windows.Controls;
using ReactiveUI;

namespace PlayStats.UI
{
    public class AddGameViewModel : ReactiveObject
    {
        private string _name;

        public AddGameViewModel()
        {
            Name = "ABC";
        }

        public string Name
        {
            get { return _name; }

            set { _name = value; this.RaisePropertyChanged(); }
        }
    }
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var strValue = value as string;
            return string.IsNullOrEmpty(strValue)
                ? new ValidationResult(false, "The string cannot be empty.")
                : ValidationResult.ValidResult;
        }
    }
}