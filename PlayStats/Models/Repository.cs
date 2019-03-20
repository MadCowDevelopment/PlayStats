using DynamicData;
using PlayStats.Data;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlayStats.Models
{
    public interface IRepository
    {
        void AddOrUpdate(PlayModel play);

        Task Load();

        IObservable<IChangeSet<GameModel, Guid>> Games { get; }

        IObservable<IChangeSet<PlayModel, Guid>> Plays { get; }
    }

    public class Repository : IRepository
    {
        private readonly IDataAccessor<GameEntity> _gameAccessor;
        private readonly IDataAccessor<PlayEntity> _playAccessor;
        private readonly IDataAccessor<LinkedGameEntity> _linkedGameAccessor;

        private SourceCache<GameModel, Guid> _games;
        private SourceCache<PlayModel, Guid> _plays;
        private SourceCache<LinkedGameModel, Guid> _linkedGames;

        public Repository(IDataAccessor<GameEntity> gameAccessor, IDataAccessor<PlayEntity> playAccessor, IDataAccessor<LinkedGameEntity> linkedGameAccessor)
        {
            _gameAccessor = gameAccessor;
            _playAccessor = playAccessor;
            _linkedGameAccessor = linkedGameAccessor;
        }

        public IObservable<IChangeSet<GameModel, Guid>> Games => _games.Connect();

        public IObservable<IChangeSet<PlayModel, Guid>> Plays => _plays.Connect();

        private IObservable<IChangeSet<LinkedGameModel, Guid>> LinkedGames => _linkedGames.Connect();

        public Task Load()
        {
            return Task.Run(() =>
            {                              
                LoadPlays();
                LoadLinkedGames();
                LoadGames();        
            });
        }
                
        public void AddOrUpdate(PlayModel play)
        {
            _plays.AddOrUpdate(play);
        }

        private void LoadPlays()
        {
            _plays = new SourceCache<PlayModel, Guid>(p => p.Id);
            foreach (var playEntity in _playAccessor.GetAll())
            {
                var play = MapPlayEntityToModel(playEntity);
                _plays.AddOrUpdate(play);
            }
        }

        private void LoadLinkedGames()
        {
            _linkedGames = new SourceCache<LinkedGameModel, Guid>(p => p.Id);
            foreach (var linkedGameEntity in _linkedGameAccessor.GetAll())
            {
                var linkedGame = MapLinkedGameEntityToModel(linkedGameEntity);
                _linkedGames.AddOrUpdate(linkedGame);
            }
        }

        private void LoadGames()
        {
            _games = new SourceCache<GameModel, Guid>(p => p.Id);
            foreach(var gameEntity in _gameAccessor.GetAll())
            {
                var game = MapGameEntityToModel(gameEntity);

                _games.AddOrUpdate(game);
            }
        }

        private PlayModel MapPlayEntityToModel(PlayEntity playEntity)
        {
            var playModel = new PlayModel(playEntity.Id, playEntity.GameId);
            playModel.Comment = playEntity.Comment;
            playModel.Date = playEntity.Date;
            playModel.Duration = playEntity.Duration;
            playModel.PlayerCount = playEntity.PlayerCount;
            
            return playModel;
        }

        private LinkedGameModel MapLinkedGameEntityToModel(LinkedGameEntity entity)
        {
            var model = new LinkedGameModel(entity.Id, entity.GameId);
            SetGameModelBaseProperties(model, entity);
            return model;
        }

        private GameModel MapGameEntityToModel(GameEntity entity)
        {
            Plays.Filter(x => x.GameId == entity.Id)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out ReadOnlyObservableCollection<PlayModel> plays)
                .Subscribe();
            LinkedGames.Filter(x => x.GameId == entity.Id)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out ReadOnlyObservableCollection<LinkedGameModel> linkedGames)
                .Subscribe();

            var model = new GameModel(entity.Id, plays, linkedGames);
            SetGameModelBaseProperties(model, entity);
            model.DesireToPlay = entity.DesireToPlay;
            model.Rating = entity.Rating;
            model.SoloMode = entity.SoloMode;
            return model;
        }

        private void SetGameModelBaseProperties(GameModelBase model, GameEntityBase entity)
        {
            model.Description = entity.Description;
            model.Designer = entity.Designer;
            model.FullName = entity.FullName;
            model.Image = entity.Image;
            model.IsDelivered = entity.IsDelivered;
            model.IsGenuine = entity.IsGenuine;
            model.Name = entity.Name;
            model.ObjectId = entity.ObjectId;
            model.Publisher = entity.Publisher;
            model.PurchasePrice = entity.PurchasePrice;
            model.SellPrice = entity.SellPrice;
            model.Thumbnail = entity.Thumbnail;
            model.WantToSell = entity.WantToSell;
            model.YearPublished = entity.YearPublished;
        }
    }
}