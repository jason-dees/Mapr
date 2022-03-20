using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapR.DataStores {
	public class AutoMapperConfiguration {
		public static IMapper  MapperConfiguration(IServiceCollection services) {
            var mapper = MapperConfiguration();
            
            services.AddSingleton<IMapper>((x) => mapper);
			return mapper;
		}

        public static IMapper MapperConfiguration()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Data.Models.IAmAGameModel, Game>().ReverseMap();
                cfg.CreateMap<Data.Models.IAmAMapModel, Map>().ReverseMap();
                cfg.CreateMap<Data.Models.IAmAMarkerModel, Marker>().ReverseMap();
                /*cfg.CreateMap<Data.Models.MapRRole, MapRRole>().ReverseMap();
                cfg.CreateMap<Data.Models.MapRUser, MapRUser>().ReverseMap();*/
            });
            return config.CreateMapper();
        }
	}
}
