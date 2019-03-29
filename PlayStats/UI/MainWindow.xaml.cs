// MainWindow class derives off ReactiveWindow which implements the IViewFor<TViewModel>
// interface using a WPF DependencyProperty. We need this to use WhenActivated extension
// method that helps us handling View and ViewModel activation and deactivation.
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;

namespace PlayStats.UI
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<MainWindowViewModel>();
            if (ViewModel == null)
            {
                throw new InvalidOperationException($"Couldn't resolve {nameof(MainWindowViewModel)}.");
            }

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Content,
                    view => view.ContentHost.ViewModel)
                    .DisposeWith(disposableRegistration);

                #region ToolBar buttons
                this.BindCommand(ViewModel,
                    viewModel => viewModel.ShowView,
                    view => view.ShowHomeButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.ShowView,
                        view => view.AddNewGameButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.ShowView,
                        view => view.AddNewPlayButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    viewModel => viewModel.ShowView,
                    view => view.ShowGameListButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                    viewModel => viewModel.ShowView,
                    view => view.ShowGameGridButton)
                    .DisposeWith(disposableRegistration);
                #endregion ToolBar buttons

                this.BindCommand(ViewModel,
                    viewModel => viewModel.Exit,
                    view => view.ExitButton)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}