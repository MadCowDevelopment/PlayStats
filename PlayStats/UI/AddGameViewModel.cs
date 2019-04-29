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
using PlayStats.Data;
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

            _availableSoloModes =
                new ReadOnlyCollection<SoloModeViewModel>(
                    ((SoloMode[]) Enum.GetValues(typeof(SoloMode))).Select(p => new SoloModeViewModel(p)).ToList());

            AddValidationRules();

            _repository.Games
                .Sort(SortExpressionComparer<GameModel>.Ascending(p => p.Name))
                .Transform(p => new AvailableGameViewModel(p.Id, p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _availableGames).Subscribe();

            IsGameChecked = true;
            IsDelivered = false;
            SelectedSoloMode = _availableSoloModes[1];
            
            var canSave = WhenDataErrorsChanged.Select(p => !p);
            Save = ReactiveCommand.CreateFromTask(SaveGame, canSave);
        }

        private void AddValidationRules()
        {
            //AddRule(new DelegateRule<AddPlayViewModel>(nameof(SelectedGame),
            //    p => p.SelectedGame != null ? string.Empty : "No game selected"));
        }

        [Reactive] public AvailableGameViewModel SelectedGame { get; set; }
        [Reactive] public bool IsGameChecked { get; set; }
        [Reactive] public bool IsExpansionChecked { get; set; }
        [Reactive] public string GameName { get; set; }
        [Reactive] public double? PurchasePrice { get; set; }
        [Reactive] public bool IsDelivered { get; set; }
        [Reactive] public SoloModeViewModel SelectedSoloMode { get; set; }

        public ICommand Save { get; }

        private readonly ReadOnlyObservableCollection<AvailableGameViewModel> _availableGames;
        public IEnumerable<AvailableGameViewModel> AvailableGames => _availableGames;

        private readonly ReadOnlyCollection<SoloModeViewModel> _availableSoloModes;
        public IEnumerable<SoloModeViewModel> AvailableSoloModes => _availableSoloModes;

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

    public class SoloModeViewModel : ReactiveObject
    {
        public SoloMode SoloMode { get; }

        public SoloModeViewModel(SoloMode soloMode)
        {
            SoloMode = soloMode;
        }

        public override string ToString()
        {
            return SoloMode.ToString();
        }
    }
}