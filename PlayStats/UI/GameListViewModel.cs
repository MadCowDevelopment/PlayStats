using DynamicData;
using PlayStats.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace PlayStats.UI
{
    public class GameListViewModel : ReactiveObject, IGameListTabViewModel
    {
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }

        private ReadOnlyObservableCollection<GameDetailsViewModel> _searchResults;
        public IEnumerable<GameDetailsViewModel> SearchResults => _searchResults;

        private ObservableAsPropertyHelper<bool> _isAvailable;
        public bool IsAvailable => _isAvailable.Value;

        public string Name { get; } = "GAME LIST";

        private readonly IRepository _repository;

        public GameListViewModel(IRepository repository)
        {
            _repository = repository;

            _repository.Load().ContinueWith(t =>
            {
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

                _isAvailable.ThrownExceptions.Subscribe(error =>Console.WriteLine(error));
            });
        }

        private Func<GameModel, bool> CreateSearchFilter(string term)
        {
            return string.IsNullOrWhiteSpace(term) ? (Func<GameModel, bool>)(game => true) : game => game.Name.ToLower().Contains(term);
        }
    }
}