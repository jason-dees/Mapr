using AutoMapper;
using MapR.CosmosStores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapR.CosmosStores {
	public class AutoMapperConfiguration {
		public static IMapper  MapperConfiguration(IServiceCollection services) {
            var mapper = MapperConfiguration();
            
            services.AddSingleton<IMapper>((x) => mapper);
			return mapper;
		}

        public static IMapper MapperConfiguration()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Data.Models.GameModel, Game>().ReverseMap();
                cfg.CreateMap<Data.Models.MapModel, Map>().ReverseMap();
                cfg.CreateMap<Data.Models.MarkerModel, Marker>().ReverseMap();
                /*cfg.CreateMap<Data.Models.MapRRole, MapRRole>().ReverseMap();
                cfg.CreateMap<Data.Models.MapRUser, MapRUser>().ReverseMap();*/
            });
            return config.CreateMapper();
        }
	}
}
