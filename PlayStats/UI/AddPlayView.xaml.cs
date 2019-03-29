using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows.Forms;

namespace PlayStats.UI
{
    /// <summary>
    /// Interaction logic for GameListView.xaml
    /// </summary>
    public partial class AddPlayView : ReactiveUserControl<AddPlayViewModel>
    {
        public AddPlayView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
                {
                    this.OneWayBind(ViewModel, 
                        vm => vm.AvailableGames, 
                        v => v.GamesComboBox.ItemsSource,
                        disposableRegistration);
                    this.Bind(ViewModel,
                        vm => vm.SelectedGame,
                        v => v.GamesComboBox.SelectedItem,
                        disposableRegistration);
                    this.Bind(ViewModel,
                        vm => vm.SelectedDate,
                        v => v.DatePicker.SelectedDate,
                        disposableRegistration);
                    this.Bind(ViewModel,
                        vm => vm.SelectedTime,
                        v => v.TimePicker.SelectedTime);
                    this.Bind(ViewModel,
                        vm => vm.PlayerCount,
                        v => v.PlayerCountTextBox.Text);
                    this.Bind(ViewModel,
                        vm => vm.Description,
                        v => v.DescriptionTextBox.Text);
                    this.BindCommand(ViewModel,
                            viewModel => viewModel.Save,
                            view => view.SaveButton)
                        .DisposeWith(disposableRegistration);
                });
        }
    }
}