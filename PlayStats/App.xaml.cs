using Autofac;
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

            // These Initialize methods will add ReactiveUI platform registrations to your container
            // They MUST be present if you override the default Locator
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            Locator.Current = resolver;

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }
}
