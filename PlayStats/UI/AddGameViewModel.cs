using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using PlayStats.Data;
using PlayStats.Models;
using PlayStats.Services;
using PlayStats.UI.Validation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.UI
{
    public class AddGameViewModel : ValidationViewModel
    {
        private readonly IRepository _repository;
        private readonly IBggService _bggService;
        private readonly INotificationService _notificationService;

        public AddGameViewModel(IRepository repository, IBggService bggService, INotificationService notificationService)
        {
            _repository = repository;
            _bggService = bggService;
            _notificationService = notificationService;

            _availableSoloModes =
                new ReadOnlyCollection<SoloModeViewModel>(
                    ((SoloMode[])Enum.GetValues(typeof(SoloMode))).Select(p => new SoloModeViewModel(p)).ToList());

            _repository.Games
                 .Sort(SortExpressionComparer<GameModel>.Ascending(p => p.Name))
                 .Transform(p => new AvailableGameViewModel(p.Id, p.Name))
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .Bind(out _availableGames).Subscribe();

            SetInitialValues();
            AddValidationRules();

            _availableBggGames = this
                .WhenAnyValue(x => x.BggGameName)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .SelectMany(SearchBggGames)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.AvailableBggGames);

            this.WhenAnyValue(x => x.SelectedBggGame)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .Select(LoadBggGameDetail)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async p => await AssignSelectedBggGameDetail(p));

            var canSave = WhenDataErrorsChanged.Select(p => !p);
            Save = ReactiveCommand.CreateFromTask(SaveGame, canSave);
        }

        [Reactive] public AvailableGameViewModel SelectedGame { get; set; }
        [Reactive] public bool IsGameChecked { get; set; }
        [Reactive] public bool IsExpansionChecked { get; set; }
        [Reactive] public string GameName { get; set; }
        [Reactive] public double? PurchasePrice { get; set; }
        [Reactive] public bool IsDelivered { get; set; }
        [Reactive] public SoloModeViewModel SelectedSoloMode { get; set; }

        [Reactive] public BggGameInfo SelectedBggGame { get; set; }
        [Reactive] public string BggGameName { get; set; }
        [Reactive] public BggGameDetail SelectedBggGameDetail { get; set; }


        public ICommand Save { get; }

        private readonly ReadOnlyObservableCollection<AvailableGameViewModel> _availableGames;
        public IEnumerable<AvailableGameViewModel> AvailableGames => _availableGames;

        private readonly ReadOnlyCollection<SoloModeViewModel> _availableSoloModes;
        public IEnumerable<SoloModeViewModel> AvailableSoloModes => _availableSoloModes;

        private readonly ObservableAsPropertyHelper<IEnumerable<BggGameInfo>> _availableBggGames;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _bggLoadLock = new object();
        public IEnumerable<BggGameInfo> AvailableBggGames => _availableBggGames.Value;

        private void SetInitialValues()
        {
            GameName = string.Empty;
            PurchasePrice = 0;
            IsGameChecked = true;
            IsDelivered = false;
            SelectedSoloMode = _availableSoloModes[1];
        }

        private void AddValidationRules()
        {
            AddRule(new DelegateRule<AddGameViewModel>(nameof(SelectedGame),
                p => IsExpansionChecked && SelectedGame == null ? "No game selected" : string.Empty));
            AddRule(new DelegateRule<AddGameViewModel>(nameof(GameName),
                p => IsGameChecked && string.IsNullOrEmpty(GameName) ? "Game needs a name" : string.Empty));
            AddRule(new DelegateRule<AddGameViewModel>(nameof(GameName),
                p => IsGameChecked && GameName != null && AvailableGames.Any(g => g.Name.ToLower().Equals(GameName.ToLower())) ? "Game with same name already exists" : string.Empty));
            AddRule(new DelegateRule<AddGameViewModel>(nameof(PurchasePrice),
                p => !PurchasePrice.HasValue || PurchasePrice.Value < 1 ? "Purchase price needs to be greater than or equal to 1" : string.Empty));
            AddRule(new DelegateRule<AddGameViewModel>(nameof(SelectedSoloMode),
                p => IsGameChecked && SelectedSoloMode == null ? "No solo mode selected" : string.Empty));
        }

        private async Task SaveGame(CancellationToken cancellationToken)
        {
            try
            {
                var game = IsGameChecked ? CreateGame() : (GameModelBase)CreateExpansion();
                game.IsDelivered = IsDelivered;
                game.PurchasePrice = PurchasePrice.Value;
                await _repository.AddOrUpdate(game);
                SetInitialValues();
            }
            catch (Exception)
            {
                _notificationService.Queue("Failed to save game!");
                return;
            }

            _notificationService.Queue("Game saved successfully.");
        }

        private async Task AssignSelectedBggGameDetail(Task<BggGameDetail> gameDetail)
        {
            if (gameDetail.IsCanceled) return;
            try
            {
                SelectedBggGameDetail = await gameDetail;
            }
            catch (TaskCanceledException)
            {
            }
        }

        private GameModel CreateGame()
        {
            var game = _repository.CreateGame();
            game.Name = GameName;
            game.SoloMode = SelectedSoloMode.SoloMode;
            return game;
        }

        private LinkedGameModel CreateExpansion()
        {
            var expansion = _repository.CreateLinkedGame(SelectedGame.Id);
            return expansion;
        }

        private Task<IEnumerable<BggGameInfo>> SearchBggGames(string term, CancellationToken token)
        {
            return _bggService.SearchGames(term, token);
        }

        private Task<BggGameDetail> LoadBggGameDetail(BggGameInfo bggGameInfo)
        {
            lock (_bggLoadLock)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                if (bggGameInfo == null) return Task.FromResult<BggGameDetail>(null);
                return _bggService.LoadGameDetails(bggGameInfo.Id, _cancellationTokenSource.Token);
            }
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