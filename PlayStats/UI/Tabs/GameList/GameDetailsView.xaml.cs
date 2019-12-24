using System.Reactive.Disposables;
using ReactiveUI;

namespace PlayStats.UI.Tabs.GameList
{
    // The class derives off ReactiveUserControl which contains the ViewModel property.
    // In our MainWindow when we register the ListBox with the collection of 
    // NugetDetailsViewModels if no ItemTemplate has been declared it will search for 
    // a class derived off IViewFor<NugetDetailsViewModel> and show that for the item.
    public partial class GameDetailsView : ReactiveUserControl<GameDetailsViewModel>
    {
        public GameDetailsView()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Id,
                    view => view.IdRun.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Name,
                    view => view.NameRun.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.OpenPage,
                    view => view.openButton)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
