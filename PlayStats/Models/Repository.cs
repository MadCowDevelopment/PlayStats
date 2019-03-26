using DynamicData;
using PlayStats.Data;
using PlayStats.Utils;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
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
            var playEntity = MapPlayModelToEntity(play);

            if (_plays.Keys.Contains(play.Id)) _playAccessor.Update(playEntity);
            else _playAccessor.Create(playEntity);

            _plays.AddOrUpdate(play);
        }

        public void AddOrUpdate(GameModel game)
        {
            var gameEntity = MapGameModelToEntity(game);

            if (_games.Keys.Contains(game.Id)) _gameAccessor.Update(gameEntity);
            else _gameAccessor.Create(gameEntity);

            _games.AddOrUpdate(game);
        }

        public void AddOrUpdate(LinkedGameModel game)
        {
            // TODO: Maybe no need?
        }

        private void LoadPlays()
        {
            _plays = new SourceCache<PlayModel, Guid>(p => p.Id);
            foreach (var playEntity in _playAccessor.GetAll())
            {
                var play = MapPlayEntityToModel(playEntity);
                _plays.AddOrUpdate(play);
            }

            Plays.WhenAnyPropertyWithAttributeChanged(typeof(ReactiveAttribute)).Subscribe(p => AddOrUpdate(p));
        }

        private void LoadLinkedGames()
        {
            _linkedGames = new SourceCache<LinkedGameModel, Guid>(p => p.Id);
            foreach (var linkedGameEntity in _linkedGameAccessor.GetAll())
            {
                var linkedGame = MapLinkedGameEntityToModel(linkedGameEntity);
                _linkedGames.AddOrUpdate(linkedGame);
            }

            LinkedGames.WhenAnyPropertyWithAttributeChanged(typeof(ReactiveAttribute)).Subscribe(p => AddOrUpdate(p));
        }

        private void LoadGames()
        {
            _games = new SourceCache<GameModel, Guid>(p => p.Id);
            foreach (var gameEntity in _gameAccessor.GetAll())
            {
                var game = MapGameEntityToModel(gameEntity);
                _games.AddOrUpdate(game);
            }

            Games.WhenAnyPropertyWithAttributeChanged(typeof(ReactiveAttribute)).Subscribe(p => AddOrUpdate(p));
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
            var model = new GameModel(entity.Id, Plays.Filter(x => x.GameId == entity.Id), LinkedGames.Filter(x => x.GameId == entity.Id));
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


        private GameEntity MapGameModelToEntity(GameModel model)
        {
            var entity = new GameEntity();
            entity.Id = model.Id;

            SetGameEntityBaseProperties(entity, model);

            entity.DesireToPlay = model.DesireToPlay;
            entity.Rating = model.Rating;
            entity.SoloMode = model.SoloMode;

            return entity;
        }

        private void SetGameEntityBaseProperties(GameEntityBase entity, GameModelBase model)
        {
            entity.Description = model.Description;
            entity.Designer = model.Designer;
            entity.FullName = model.FullName;
            entity.Image = model.Image;
            entity.IsDelivered = model.IsDelivered;
            entity.IsGenuine = model.IsGenuine;
            entity.Name = model.Name;
            entity.ObjectId = model.ObjectId;
            entity.Publisher = model.Publisher;
            entity.PurchasePrice = model.PurchasePrice;
            entity.SellPrice = model.SellPrice;
            entity.Thumbnail = model.Thumbnail;
            entity.WantToSell = model.WantToSell;
            entity.YearPublished = model.YearPublished;
        }

        private PlayEntity MapPlayModelToEntity(PlayModel model)
        {
            var entity = new PlayEntity();
            entity.Id = model.Id;
            entity.Comment = model.Comment;
            entity.Date = model.Date;
            entity.Duration = model.Duration;
            entity.PlayerCount = model.PlayerCount;
            return entity;
        }
    }
}