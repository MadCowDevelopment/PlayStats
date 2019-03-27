using DynamicData;
using PlayStats.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.UI
{
    public class GameListViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<GameDetailsViewModel> _searchResults;
        public IEnumerable<GameDetailsViewModel> SearchResults => _searchResults;

        private readonly ObservableAsPropertyHelper<bool> _isAvailable;
        public bool IsAvailable => _isAvailable.Value;

        private readonly IRepository _repository;

        public GameListViewModel(IRepository repository)
        {
            _repository = repository;

            var dynamicFilter = this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Select(term => term?.Trim().ToLower())
                .DistinctUntilChanged()
                .Select(CreateSearchFilter);

            var searchResultObservable = _repository.Games
                .Filter(dynamicFilter)
                .Transform(p => new GameDetailsViewModel(p));

            searchResultObservable
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _searchResults).Subscribe();

            _isAvailable = searchResultObservable
                .Select(x => x.Any())
                .ToProperty(this, x => x.IsAvailable);

            _isAvailable.ThrownExceptions.Subscribe(Console.WriteLine);
        }

        [Reactive] public string SearchTerm { get; set; }

        private Func<GameModel, bool> CreateSearchFilter(string term)
        {
            return string.IsNullOrWhiteSpace(term) ? (Func<GameModel, bool>)(game => true) : game => game.Name.ToLower().Contains(term);
        }
    }
}