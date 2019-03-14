// MainWindow class derives off ReactiveWindow which implements the IViewFor<TViewModel>
// interface using a WPF DependencyProperty. We need this to use WhenActivated extension
// method that helps us handling View and ViewModel activation and deactivation.
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;

namespace PlayStats
{
    // This class wraps out NuGet model object into a ViewModel and allows
    // us to have a ReactiveCommand to open the NuGet package URL.
    public class NugetDetailsViewModel : ReactiveObject
    {
        public NugetDetailsViewModel()
        {
            OpenPage = ReactiveCommand.Create(() => { Process.Start(ProjectUrl.ToString()); });
        }

        public Uri IconUrl => new Uri("https://git.io/fAlfh");
        public string Description => "Description";
        public Uri ProjectUrl => new Uri("https://git.io/fAlfh");
        public string Title => "Title";

        // ReactiveCommand allows us to execute logic without exposing any of the 
        // implementation details with the View. The generic parameters are the 
        // input into the command and it's output. In our case we don't have any 
        // input or output so we use Unit which in Reactive speak means a void type.
        public ReactiveCommand<Unit, Unit> OpenPage { get; }
    }
}