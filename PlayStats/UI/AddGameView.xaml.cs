using System.Linq;
using ReactiveUI;
using System.Reactive.Disposables;

namespace PlayStats.UI
{
    /// <summary>
    /// Interaction logic for GameListView.xaml
    /// </summary>
    public partial class AddGameView : ReactiveUserControl<AddGameViewModel>
    {
        public AddGameView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                DataContext = ViewModel;

                this.Bind(ViewModel,
                    vm => vm.IsGameChecked,
                    v => v.GameRadioButton.IsChecked);
                this.Bind(ViewModel,
                    vm => vm.IsExpansionChecked,
                    v => v.ExpansionRadioButton.IsChecked);

                this.OneWayBind(ViewModel,
                    vm => vm.AvailableGames,
                    v => v.GamesComboBox.ItemsSource,
                    disposableRegistration);

                this.Bind(ViewModel,
                    vm => vm.IsDelivered,
                    v => v.IsDeliveredCheckBox.IsChecked);

                this.Bind(ViewModel,
                    vm => vm.SelectedSoloMode,
                    v => v.SoloModeComboBox.SelectedItem);
                this.OneWayBind(ViewModel,
                    vm => vm.AvailableSoloModes,
                    v => v.SoloModeComboBox.ItemsSource,
                    disposableRegistration);

                this.Bind(ViewModel,
                    vm => vm.SelectedBggGame,
                    v => v.AvailableBggGames.SelectedItem);

                this.Bind(ViewModel,
                    vm => vm.BggGameName,
                    v => v.BggGameNameTextBox.Text);
                this.OneWayBind(ViewModel,
                    vm => vm.AvailableBggGames,
                    v => v.AvailableBggGames.ItemsSource,
                    disposableRegistration);

                this.OneWayBind(ViewModel,
                    vm => vm.SelectedBggGameDetail.FullName,
                    v => v.BggFullNameTextBox.Text);
                this.OneWayBind(ViewModel,
                    vm => vm.SelectedBggGameDetail.YearPublished,
                    v => v.BggYearPublishedTextBox.Text);
                this.OneWayBind(ViewModel,
                    vm => vm.SelectedBggGameDetail.Designers,
                    v => v.BggDesignersTextBox.Text,
                    d => string.Join(", ", d.Select(x => x.Name)));
                this.OneWayBind(ViewModel,
                    vm => vm.SelectedBggGameDetail.Publishers,
                    v => v.BggPublishersTextBox.Text,
                    p => string.Join(", ", p.Select(x => x.Name)));
                this.OneWayBind(ViewModel,
                    vm => vm.SelectedBggGameDetail.Description,
                    v => v.BggDescriptionTextBox.Text);

                this.OneWayBind(ViewModel,
                    vm => vm.IsGameChecked,
                    v => v.GameNameTextBox.Visibility);
                this.OneWayBind(ViewModel,
                    vm => vm.IsExpansionChecked,
                    v => v.GamesComboBox.Visibility);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.Save,
                        view => view.SaveButton)
                    .DisposeWith(disposableRegistration);
        });
        }
    }
}