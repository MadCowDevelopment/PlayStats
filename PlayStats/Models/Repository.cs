using AutoMapper;
using DynamicData;
using PlayStats.Data;
using PlayStats.Utils;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlayStats.Models
{
    public interface IRepository
    {
        PlayModel CreatePlay(Guid gameId);
        GameModel CreateGame();
        LinkedGameModel CreateLinkedGame(Guid gameId);

        Task AddOrUpdate(PlayModel play);
        Task AddOrUpdate(GameModelBase gameModelBase);

        Task Load();

        IObservable<IChangeSet<GameModel, Guid>> Games { get; }

        IObservable<IChangeSet<PlayModel, Guid>> Plays { get; }
    }

    public class Repository : IRepository
    {
        private readonly IMapper _mapper;
        private readonly IDataAccessor<GameEntity> _gameAccessor;
        private readonly IDataAccessor<PlayEntity> _playAccessor;
        private readonly IDataAccessor<LinkedGameEntity> _linkedGameAccessor;

        private SourceCache<GameModel, Guid> _games;
        private SourceCache<PlayModel, Guid> _plays;
        private SourceCache<LinkedGameModel, Guid> _linkedGames;

        public Repository(
            IMapper mapper,
            IDataAccessor<GameEntity> gameAccessor,
            IDataAccessor<PlayEntity> playAccessor,
            IDataAccessor<LinkedGameEntity> linkedGameAccessor)
        {
            _mapper = mapper;
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

        public PlayModel CreatePlay(Guid gameId)
        {
            return new PlayModel(Guid.NewGuid(), gameId);
        }

        public GameModel CreateGame()
        {
            var gameId = Guid.NewGuid();
            return new GameModel(gameId, Plays.Filter(x => x.GameId == gameId), LinkedGames.Filter(x => x.GameId == gameId));
        }

        public LinkedGameModel CreateLinkedGame(Guid gameId)
        {
            return new LinkedGameModel(Guid.NewGuid(), gameId);
        }

        public Task AddOrUpdate(PlayModel play)
        {
            return Task.Factory.StartNew(() =>
            {
                var playEntity = _mapper.Map<PlayEntity>(play);

                if (_plays.Keys.Contains(play.Id)) _playAccessor.Update(playEntity);
                else _playAccessor.Create(playEntity);

                _plays.AddOrUpdate(play);
            });
        }

        public Task AddOrUpdate(GameModelBase gameModelBase)
        {
            if (gameModelBase is GameModel game)
            {
                return AddOrUpdate(game);
            }

            if (gameModelBase is LinkedGameModel expansion)
            {
                return AddOrUpdate(expansion);
            }

            throw new InvalidOperationException($"Type {gameModelBase.GetType()} is not supported.");
        }

        public Task AddOrUpdate(GameModel game)
        {
            return Task.Factory.StartNew(() =>
            {
                var gameEntity = _mapper.Map<GameEntity>(game);

                if (_games.Keys.Contains(game.Id)) _gameAccessor.Update(gameEntity);
                else _gameAccessor.Create(gameEntity);

                _games.AddOrUpdate(game);
            });
        }

        public Task AddOrUpdate(LinkedGameModel expansion)
        {
            return Task.Factory.StartNew(() =>
            {
                var linkedGameEntity = _mapper.Map<LinkedGameEntity>(expansion);

                if (_linkedGames.Keys.Contains(expansion.Id)) _linkedGameAccessor.Update(linkedGameEntity);
                else _linkedGameAccessor.Create(linkedGameEntity);

                _linkedGames.AddOrUpdate(expansion);
            });
        }

        private void LoadPlays()
        {
            _plays = new SourceCache<PlayModel, Guid>(p => p.Id);
            foreach (var entity in _playAccessor.GetAll())
            {
                var play = _mapper.Map<PlayModel>(entity);

                _plays.AddOrUpdate(play);
            }

            Plays.WhenAnyPropertyWithAttributeChanged(typeof(ReactiveAttribute))
                .Subscribe(p => AddOrUpdate(p).Wait());
        }

        private void LoadLinkedGames()
        {
            _linkedGames = new SourceCache<LinkedGameModel, Guid>(p => p.Id);
            foreach (var linkedGameEntity in _linkedGameAccessor.GetAll())
            {
                var linkedGame = _mapper.Map<LinkedGameModel>(linkedGameEntity);
                _linkedGames.AddOrUpdate(linkedGame);
            }

            LinkedGames.WhenAnyPropertyWithAttributeChanged(typeof(ReactiveAttribute))
                .Subscribe(p => AddOrUpdate(p).Wait());
        }

        private void LoadGames()
        {
            _games = new SourceCache<GameModel, Guid>(p => p.Id);
            foreach (var entity in _gameAccessor.GetAll())
            {
                var game = _mapper.Map<GameModel>(entity, opt =>
                {
                    opt.Items[nameof(Plays)] = Plays.Filter(x => x.GameId == entity.Id);
                    opt.Items[nameof(LinkedGames)] = LinkedGames.Filter(x => x.GameId == entity.Id);
                });
                _games.AddOrUpdate(game);
            }

            Games.WhenAnyPropertyWithAttributeChanged(typeof(ReactiveAttribute))
                .Subscribe(p => AddOrUpdate(p).Wait());
        }
    }
}