using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using PlayStats.Services;
using PlayStats.UI.Dialogs.Update;
using ReactiveUI;

namespace PlayStats.UI.Tabs.Home
{
    public class HomeViewModel : ReactiveObject
    {
        private readonly IDialogService _dialogService;

        public HomeViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Task.Delay(2000).ContinueWith(async t =>
                {
                    try
                    {
                        await _dialogService.Show<UpdateViewModel>();
                    }
                    catch (System.Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                },
                CancellationToken.None, TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}