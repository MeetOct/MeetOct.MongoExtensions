using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace MeetOct.MongoExtensions
{
    public static class MongoServiceCollectionExtensions
    {
		public static IServiceCollection AddMongoDB(this IServiceCollection services,string connectionString)
		{
			services.AddSingleton<MongoManager>();
			services.AddSingleton<IMongoClient, MongoClient>(provider=>provider.GetService<MongoManager>().GetClient(connectionString));

			services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
			return services;
		}
	}
}
