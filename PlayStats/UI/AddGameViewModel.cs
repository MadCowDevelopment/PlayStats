using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using DynamicData;
using DynamicData.Binding;
using PlayStats.Models;
using PlayStats.Services;
using PlayStats.UI.Validation;
using PlayStats.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.UI
{
    public class AddGameViewModel : ValidationViewModel
    {
        private readonly IRepository _repository;
        private readonly INotificationService _notificationService;

        public AddGameViewModel(IRepository repository, INotificationService notificationService, IMapper mapper)
        {
            _repository = repository;
            _notificationService = notificationService;

            AddValidationRules();

            _repository.Games
                .Sort(SortExpressionComparer<GameModel>.Ascending(p => p.Name))
                .Transform(p => new AvailableGameViewModel(p.Id, p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _availableGames).Subscribe();
            
            var canSave = WhenDataErrorsChanged.Select(p => !p);
            Save = ReactiveCommand.CreateFromTask(SaveGame, canSave);
        }

        private void AddValidationRules()
        {
            //AddRule(new DelegateRule<AddPlayViewModel>(nameof(SelectedGame),
            //    p => p.SelectedGame != null ? string.Empty : "No game selected"));
        }

        [Reactive] public AvailableGameViewModel SelectedGame { get; set; }
     
        public ICommand Save { get; }
     
        private readonly ReadOnlyObservableCollection<AvailableGameViewModel> _availableGames;
        public IEnumerable<AvailableGameViewModel> AvailableGames => _availableGames;

        private async Task SaveGame(CancellationToken cancellationToken)
        {
            try
            {
                //var play = _repository.CreatePlay(SelectedGame.Id);
                //await _repository.AddOrUpdate(play);
            }
            catch (Exception)
            {
                _notificationService.Queue("Failed to save game!");
                return;
            }

            _notificationService.Queue("Game saved successfully.");
        }
    }
}