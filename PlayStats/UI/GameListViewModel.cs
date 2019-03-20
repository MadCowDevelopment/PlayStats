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
    public class GameListViewModel : ReactiveObject
    {
        // In ReactiveUI, this is the syntax to declare a read-write property
        // that will notify Observers, as well as WPF, that a property has 
        // changed. If we declared this as a normal property, we couldn't tell 
        // when it has changed!
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }

        // Here's the interesting part: In ReactiveUI, we can take IObservables
        // and "pipe" them to a Property - whenever the Observable yields a new
        // value, we will notify ReactiveObject that the property has changed.
        // 
        // To do this, we have a class called ObservableAsPropertyHelper - this
        // class subscribes to an Observable and stores a copy of the latest value.
        // It also runs an action whenever the property changes, usually calling
        // ReactiveObject's RaisePropertyChanged.
        private ReadOnlyObservableCollection<GameDetailsViewModel> _searchResults;

        private IObservable<IChangeSet<GameDetailsViewModel, Guid>> _gameDetails;

        public IEnumerable<GameDetailsViewModel> SearchResults => _searchResults;

        // Here, we want to create a property to represent when the application 
        // is performing a search (i.e. when to show the "spinner" control that 
        // lets the user know that the app is busy). We also declare this property
        // to be the result of an Observable (i.e. its value is derived from 
        // some other property)
        private ObservableAsPropertyHelper<bool> _isAvailable;
        public bool IsAvailable => _isAvailable.Value;

        private readonly IRepository _repository;

        public GameListViewModel(IRepository repository)
        {
            _repository = repository;

            _repository.Load().ContinueWith(t =>
            {
                var dynamicFilter = this.WhenAnyValue(x => x.SearchTerm)
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    .Select(term => term?.Trim())
                    .DistinctUntilChanged()
                    .Select(CreateFilterPredicate);

                var searchResultObservable = _repository.Games.Filter(dynamicFilter)
                    .Transform(p => new GameDetailsViewModel(p));                    

                var searchResults = searchResultObservable
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .AsObservableCache();

                searchResults.Connect().Bind(out _searchResults).Subscribe();

                _isAvailable = dynamicFilter
                    .Select(x => true)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, x => x.IsAvailable);

                _isAvailable.ThrownExceptions.Subscribe(error =>Console.WriteLine(error));
            });
        }

        private Func<GameModel, bool> CreateFilterPredicate(string term)
        {
            return string.IsNullOrWhiteSpace(term) ? (Func<GameModel, bool>)(game => true) : game => game.Name.Contains(term);
        }
    }
}