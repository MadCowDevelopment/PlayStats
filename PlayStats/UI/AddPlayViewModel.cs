using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.UI
{
    public class AddPlayViewModel : ValidationViewModel
    {
        private readonly IRepository _repository;
        private readonly INotificationService _notificationService;

        public AddPlayViewModel(IRepository repository, INotificationService notificationService, IMapper mapper)
        {
            _repository = repository;
            _notificationService = notificationService;

            AddValidationRules();

            repository.Games
                .Sort(SortExpressionComparer<GameModel>.Ascending(p => p.Name))
                .Transform(p => new AvailableGameViewModel(p.Id, p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _availableGames).Subscribe();

            var selectedGameFilter = this.WhenAnyValue(x => x.SelectedGame)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .DistinctUntilChanged()
                .Select(p=>p?.Id)
                .Select(CreateRecentPlayFilter);

            _repository.Plays
                .Filter(selectedGameFilter)
                .Sort(SortExpressionComparer<PlayModel>.Descending(p=>p.Date))
                .Top(15)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _recentPlays).Subscribe();

            var canSave = WhenDataErrorsChanged.Select(p => !p);
            Save = ReactiveCommand.CreateFromTask(SavePlay, canSave);

            SelectedDate = DateTime.Today;
            PlayerCount = 1;
        }

        private Func<PlayModel, bool> CreateRecentPlayFilter(Guid? gameId)
        {
            return gameId == null ? (Func<PlayModel, bool>) (play => false) : play => play.GameId.Equals(gameId);
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

        public ICommand Save { get; }

        private readonly ReadOnlyObservableCollection<AvailableGameViewModel> _availableGames;
        public IEnumerable<AvailableGameViewModel> AvailableGames => _availableGames;

        private readonly ReadOnlyObservableCollection<PlayModel> _recentPlays;
        public IEnumerable<PlayModel> RecentPlays => _recentPlays;

        private async Task SavePlay(CancellationToken cancellationToken)
        {
            try
            {
                var play = _repository.CreatePlay(SelectedGame.Id);
                play.Comment = Comment;
                play.Date = SelectedDate.Value;
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

            Comment = string.Empty;
            SelectedTime = null;
        }
    }

    public class AvailableGameViewModel : ReactiveObject
    {
        public AvailableGameViewModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}