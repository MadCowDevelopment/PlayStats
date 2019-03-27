using ReactiveUI;
using Splat;
using System.Reflection;
using System.Windows;

namespace PlayStats
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var registrar = new AutofacDependencyRegistrar();
            var resolver = new AutofacDependencyResolver(registrar.Build());

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            Locator.Current = resolver;
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }
}
