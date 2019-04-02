using AutoMapper;
using DynamicData;
using PlayStats.Data;
using PlayStats.Models;
using System;
using PlayStats.UI;

namespace PlayStats
{
    static class AutoMapperConfigurator
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GameEntity, GameModel>()
                    .ForCtorParam("id", opt => opt.MapFrom(src => src.Id))
                    .ForCtorParam("plays", opt => opt.MapFrom(ResolvePlays))
                    .ForCtorParam("linkedGames", opt => opt.MapFrom(ResolveLinkedGames))
                    .ForMember(dest => dest.Plays, opt => opt.Ignore())
                    .ForMember(dest => dest.LinkedGames, opt => opt.Ignore());

                cfg.CreateMap<PlayEntity, PlayModel>()
                    .ForCtorParam("id", opt => opt.MapFrom(src => src.Id));
                cfg.CreateMap<LinkedGameEntity, LinkedGameModel>();

                cfg.CreateMap<GameModel, GameEntity>();
                cfg.CreateMap<PlayModel, PlayEntity>();
                cfg.CreateMap<LinkedGameModel, LinkedGameEntity>();
            });

            config.AssertConfigurationIsValid();

            return config.CreateMapper();
        }

        private static IObservable<IChangeSet<PlayModel, Guid>> ResolvePlays(GameEntity entity, ResolutionContext context)
        {
            return context.Items[nameof(GameModel.Plays)] as IObservable<IChangeSet<PlayModel, Guid>>;
        }

        private static IObservable<IChangeSet<LinkedGameModel, Guid>> ResolveLinkedGames(GameEntity entity, ResolutionContext context)
        {
            return context.Items[nameof(GameModel.LinkedGames)] as IObservable<IChangeSet<LinkedGameModel, Guid>>;
        }
    }
}
