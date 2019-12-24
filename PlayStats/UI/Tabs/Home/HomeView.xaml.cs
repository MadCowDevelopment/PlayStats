using ReactiveUI;

namespace PlayStats.UI.Tabs.Home
{
    /// <summary>
    /// Interaction logic for GameListView.xaml
    /// </summary>
    public partial class HomeView : ReactiveUserControl<HomeViewModel>
    {
        public HomeView()
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