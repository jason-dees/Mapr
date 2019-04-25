using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapR.DataStores.Configuration {
	public class AutoMapperConfiguration {
		public static IAmDataStoreMapper  MapperConfiguration(IServiceCollection services) {
			var config = new MapperConfiguration(cfg => {
				cfg.CreateMap<Data.Models.GameModel, GameModel>();
				cfg.CreateMap<Data.Models.MapModel, MapModel>();
				cfg.CreateMap<Data.Models.MapRRole, MapRRole>();
				cfg.CreateMap<Data.Models.MapRUser, MapRUser>();
				cfg.CreateMap<Data.Models.MarkerModel, MarkerModel>();
			});
			var mapper = config.CreateMapper() as IAmDataStoreMapper;

			//services.AddScoped<IAmDataStoreMapper>((x) => mapper);
			return mapper;
		}
	}

	public interface IAmDataStoreMapper : IMapper { }
}
