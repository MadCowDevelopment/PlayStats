using System;
using System.Threading.Tasks;
using PlayStats.Models;
using PlayStats.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.UI.Dialogs.AppStartup
{
    public class ApplicationStartupViewModel : ReactiveObject
    {
        private readonly IApplicationUpdateService _updateService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly IRepository _repository;

        public ApplicationStartupViewModel(IRepository repository, IApplicationUpdateService updateService, IViewModelFactory viewModelFactory, IDialogService dialogService)
        {
            _updateService = updateService;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _repository = repository;
        }

        public async Task Run()
        {
            CurrentOperation = "Checking for updates...";
            var updateAvailable = await _updateService.CheckForUpdates();
            if (updateAvailable)
            {
                CurrentOperation = "Updating application...";
                await Update();
            }

            CurrentOperation = "Initializing game data...";
            await _repository.Load();
        }

        private async Task Update()
        {
            await _updateService.Update();
            await _dialogService.MessageBox("Update finished",
                "The application was updated. Please click 'Ok' to restart.");

            Environment.Exit(1);
        }

        [Reactive] public string CurrentOperation { get; set; }
    }
}
