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
    public class AddPlayViewModel : ValidationViewModel
    {
        private const int DefaultNumberOfRecentPlays = 15;

        private readonly IRepository _repository;
        private readonly INotificationService _notificationService;

        public AddPlayViewModel(IRepository repository, INotificationService notificationService, IMapper mapper)
        {
            _repository = repository;
            _notificationService = notificationService;

            AddValidationRules();

            _repository.Games
                .Sort(SortExpressionComparer<GameModel>.Ascending(p => p.Name))
                .Transform(p => new AvailableGameViewModel(p.Id, p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _availableGames).Subscribe();

            var selectedGameFilter = this.WhenAnyValue(x => x.SelectedGame)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .DistinctUntilChanged()
                .Select(p => p?.Id)
                .Select(CreateRecentPlayFilter);

            this.WhenAnyValue(x => x.SelectedGame)
                .Subscribe(p => NumberOfRecentPlays = DefaultNumberOfRecentPlays);

            var whenTopChanges = this.WhenAnyValue(x => x.NumberOfRecentPlays)
                .Select(p => new TopRequest(NumberOfRecentPlays));

            _repository.Plays
                .Filter(selectedGameFilter)
                .Sort(SortExpressionComparer<PlayModel>.Descending(p => p.Date))
                .Virtualise(whenTopChanges)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _recentPlays).Subscribe();

            

            var canSave = WhenDataErrorsChanged.Select(p => !p);
            Save = ReactiveCommand.CreateFromTask(SavePlay, canSave);

            var canLoadMore = _recentPlays.ToObservableChangeSet(p => p.Id).Select(p => _recentPlays.Count == NumberOfRecentPlays)
                .ObserveOn(RxApp.MainThreadScheduler);
            LoadMore = ReactiveCommand.Create(() => NumberOfRecentPlays += 15, canLoadMore);

            _recentPlays.ToObservableChangeSet(p=>p.Id)
                .StartWithEmpty()
                .Select(p => string.Format($"{_recentPlays.Count} most recent plays"))
                .ToPropertyEx(this, x => x.RecentPlaysHeader);

            SelectedDate = DateTime.Today;
            PlayerCount = 1;
        }

        private Func<PlayModel, bool> CreateRecentPlayFilter(Guid? gameId)
        {
            return gameId == null ? (Func<PlayModel, bool>)(play => false) : play => play.GameId.Equals(gameId);
        }

        private void AddValidationRules()
        {
            AddRule(new DelegateRule<AddPlayViewModel>(nameof(SelectedGame),
                p => p.SelectedGame != null ? string.Empty : "No game selected"));
            AddRule(new DelegateRule<AddPlayViewModel>(nameof(SelectedDate),
                p =>
                {
                    if (p.SelectedDate == null) return "No date selected";
                    if (p.SelectedDate > DateTime.Today) return "Selected date is in the future";
                    return string.Empty;
                }));
            AddRule(new DelegateRule<AddPlayViewModel>(nameof(SelectedTime),
                p => p.SelectedTime != null ? string.Empty : "No duration selected"));
            AddRule(new DelegateRule<AddPlayViewModel>(nameof(PlayerCount),
                p => p.PlayerCount.HasValue && p.PlayerCount.Value > 0 ? string.Empty : "Player count must be 1+"));
        }

        [Reactive] public AvailableGameViewModel SelectedGame { get; set; }
        [Reactive] public DateTime? SelectedDate { get; set; }
        [Reactive] public DateTime? SelectedTime { get; set; }
        [Reactive] public int? PlayerCount { get; set; }
        [Reactive] public string Comment { get; set; }

        [Reactive] public int NumberOfRecentPlays { get; set; } = DefaultNumberOfRecentPlays;

        public ICommand Save { get; }
        public ICommand LoadMore { get; }

        private readonly ReadOnlyObservableCollection<AvailableGameViewModel> _availableGames;
        public IEnumerable<AvailableGameViewModel> AvailableGames => _availableGames;

        private readonly ReadOnlyObservableCollection<PlayModel> _recentPlays;
        public IEnumerable<PlayModel> RecentPlays => _recentPlays;

        public string RecentPlaysHeader { [ObservableAsProperty] get; }

        public PlayModel MostRecentPlay => _recentPlays.FirstOrDefault();

        private async Task SavePlay(CancellationToken cancellationToken)
        {
            try
            {
                var play = _repository.CreatePlay(SelectedGame.Id);
                play.Comment = Comment;

                if (MostRecentPlay != null && MostRecentPlay.Date.IsSameDay(SelectedDate.Value))
                {
                    play.Date = MostRecentPlay.Date.AddMilliseconds(1);
                }
                else
                {
                    play.Date = SelectedDate.Value;
                }
                
                play.Duration = new TimeSpan(SelectedTime.Value.Hour, SelectedTime.Value.Minute, 0);
                play.PlayerCount = PlayerCount.Value;

                await _repository.AddOrUpdate(play);
            }
            catch (Exception)
            {
                _notificationService.Queue("Failed to save play!");
                return;
            }

            _notificationService.Queue("Play saved successfully.");
        }
    }
}