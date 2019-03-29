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
                //this.OneWayBind(ViewModel,
                //        viewModel => viewModel.IsAvailable,
                //        view => view.searchResultsListBox.Visibility)
                //        .DisposeWith(disposableRegistration);
            });
        }
    }
}