using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace PlayStats.Services
{
    public interface IDialogService
    {
        Task Show<TViewModel>() where TViewModel : ReactiveObject;
    }

    public class DialogService : IDialogService
    {
        private readonly IViewModelFactory _factory;

        public DialogService(IViewModelFactory factory)
        {
            _factory = factory;
        }

        public Task Show<TViewModel>() where TViewModel : ReactiveObject
        {
            var vm = _factory.Create<TViewModel>();
            return DialogHost.Show(vm);
        }
    }
}
