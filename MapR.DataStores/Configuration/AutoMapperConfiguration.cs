﻿using AutoMapper;
using MapR.DataStores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapR.DataStores.Configuration {
	public class AutoMapperConfiguration {
		public static IMapper  MapperConfiguration(IServiceCollection services) {
			var config = new MapperConfiguration(cfg => {
				cfg.CreateMap<Data.Models.GameModel, GameModel>().ReverseMap();
				cfg.CreateMap<Data.Models.MapModel, MapModel>().ReverseMap();
				cfg.CreateMap<Data.Models.MapRRole, MapRRole>().ReverseMap();
				cfg.CreateMap<Data.Models.MapRUser, MapRUser>().ReverseMap();
				cfg.CreateMap<Data.Models.MarkerModel, MarkerModel>().ReverseMap();
			});
			var mapper = config.CreateMapper();

			services.AddSingleton<IMapper>((x) => mapper);
			return mapper;
		}
	}
}