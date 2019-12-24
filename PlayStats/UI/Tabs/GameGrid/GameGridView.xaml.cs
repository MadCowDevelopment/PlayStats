using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace PlayStats.UI.Tabs.GameGrid
{
    /// <summary>
    /// Interaction logic for GameListView.xaml
    /// </summary>
    public partial class GameGridView : ReactiveUserControl<GameGridViewModel>
    {
        public GameGridView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Games,
                        view => view.GamesGrid.ItemsSource)
                        .DisposeWith(disposableRegistration);
            });
        }

        private void DataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }
    }
}