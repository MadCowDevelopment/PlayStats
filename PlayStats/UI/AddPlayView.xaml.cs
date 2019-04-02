using ReactiveUI;
using System.Reactive.Disposables;

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
                // This is necessary to make bindings in XAML work for validation.
                DataContext = ViewModel;

                this.OneWayBind(ViewModel,
                        vm => vm.AvailableGames,
                        v => v.GamesComboBox.ItemsSource,
                        disposableRegistration);
                this.OneWayBind(ViewModel,
                    vm => vm.RecentPlays,
                    v => v.RecentPlaysGrid.ItemsSource);

                this.Bind(ViewModel,
                    vm => vm.Comment,
                    v => v.CommentTextBox.Text);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.Save,
                        view => view.SaveButton)
                    .DisposeWith(disposableRegistration);

                GamesComboBox.Focus();
            });
        }
    }
}