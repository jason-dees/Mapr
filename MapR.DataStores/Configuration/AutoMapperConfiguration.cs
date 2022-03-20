using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapR.DataStores.Configuration {
	public class AutoMapperConfiguration {
		public static IMapper MapperConfiguration(IServiceCollection services) {
            var mapper = MapperConfiguration();
            
            services.AddSingleton<IMapper>((x) => mapper);
			return mapper;
		}

        public static IMapper MapperConfiguration()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Data.Models.IAmAGameModel, GameModel>().ReverseMap();
                cfg.CreateMap<Data.Models.IAmAMapModel, MapModel>().ReverseMap();
                cfg.CreateMap<Data.Models.MapRRole, MapRRole>().ReverseMap();
                cfg.CreateMap<Data.Models.MapRUser, MapRUser>().ReverseMap();
                cfg.CreateMap<Data.Models.IAmAMarkerModel, MarkerModel>().ReverseMap();
            });
            return config.CreateMapper();
        }
	}
}
