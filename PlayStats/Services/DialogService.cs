using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using PlayStats.UI.Dialogs.MessageBox;
using ReactiveUI;

namespace PlayStats.Services
{
    public interface IDialogService
    {
        Task Show<TViewModel>() where TViewModel : ReactiveObject;
        Task<MessageBoxResult> MessageBox(string title, string message);
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
            var v = (UserControl) Activator.CreateInstance(typeof(TViewModel).Assembly.FullName,
                typeof(TViewModel).FullName.Replace("ViewModel", "View")).Unwrap();
            var vm = _factory.Create<TViewModel>();
            v.DataContext = vm;
            return DialogHost.Show(v);
        }

        public async Task<MessageBoxResult> MessageBox(string title, string message)
        {
            var v = new MessageBoxView();
            var vm = _factory.Create<MessageBoxViewModel>();
            v.DataContext = vm;
            return (MessageBoxResult) await DialogHost.Show(v);
        }
    }
}
