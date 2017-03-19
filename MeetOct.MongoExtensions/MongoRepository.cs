using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MeetOct.MongoExtensions
{
	public class MongoRepository<Entity> : IMongoRepository<Entity> where Entity : MongoEntity
	{
		private IMongoCollection<Entity> mongoCollection;
		public MongoRepository(IMongoClient client)
		{
			var typeInfo = typeof(Entity).GetTypeInfo();
			var targetType = typeof(MongoAttribute);
			if (!typeInfo.IsDefined(targetType, false))
			{
				throw new Exception("实体未配置Mongo特性");
			}
			var attr = typeInfo.GetCustomAttribute(targetType) as MongoAttribute;
			var database = client.GetDatabase(attr.Database);
			mongoCollection = database.GetCollection<Entity>(attr.Collection);
		}

		public async Task InsertOneAsync(Entity entity)
		{
			await mongoCollection.InsertOneAsync(entity);
		}

		public async Task InsertListAsync(List<Entity> entity)
		{
			await mongoCollection.InsertManyAsync(entity);
		}

		public async Task<long> DeleteOneAsync(Expression<Func<Entity, bool>> where)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			return (await mongoCollection.DeleteOneAsync(filter)).DeletedCount;
		}

		public async Task<long> DeleteListAsync(Expression<Func<Entity, bool>> where)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			return (await mongoCollection.DeleteManyAsync(filter)).DeletedCount;
		}

        public async Task UpdateOneAsync(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update)
        {
            var filter = Builders<Entity>.Filter.Where(where);
            var builder = Builders<Entity>.Update;
            List<UpdateDefinition<Entity>> updates = new List<UpdateDefinition<Entity>>();
            foreach (var item in update)
            {
                updates.Add(builder.Set(item.Key, item.Value));
            }
            await mongoCollection.UpdateOneAsync(filter, Builders<Entity>.Update.Combine(updates));
        }

        public async Task<Entity> FindOneAndUpdateAsync(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update)
        {
            var filter = Builders<Entity>.Filter.Where(where);
            var builder = Builders<Entity>.Update;
            List<UpdateDefinition<Entity>> updates = new List<UpdateDefinition<Entity>>();
            foreach (var item in update)
            {
                updates.Add(builder.Set(item.Key, item.Value));
            }
           return await mongoCollection.FindOneAndUpdateAsync(filter, Builders<Entity>.Update.Combine(updates));
        }

        public async Task UpdateListAsync(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			var builder = Builders<Entity>.Update;
			List<UpdateDefinition<Entity>> updates = new List<UpdateDefinition<Entity>>();
			foreach (var item in update)
			{
				updates.Add(builder.Set(item.Key, item.Value));
			}
			await mongoCollection.UpdateManyAsync(filter, Builders<Entity>.Update.Combine(updates));
		}


		public async Task<Entity> FindFirstOneAsync(Expression<Func<Entity, bool>> where)
		{
			return (await mongoCollection.FindAsync(Builders<Entity>.Filter.Where(where)))?.FirstOrDefault();
		}

		public async Task<List<Entity>> FindListAsync(Expression<Func<Entity, bool>> where)
		{
			return (await mongoCollection.FindAsync(Builders<Entity>.Filter.Where(where)))?.ToList();
		}

		public async Task<List<Entity>> PageListAsync(int index, int size, Expression<Func<Entity, bool>> where)
		{
			return await PageListAsync(index, size, where, null);
		}

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="index"></param>
		/// <param name="size"></param>
		/// <param name="where"></param>
		/// <param name="sort">排序方式，key代表排序字段，value 代表是否asc</param>
		/// <returns></returns>
		public async Task<List<Entity>> PageListAsync(int index, int size, Expression<Func<Entity, bool>> where, Dictionary<string, bool> sort = null)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			if (sort == null || !sort.Any())
			{
				return mongoCollection.Find(filter).Skip((index - 1) * size).Limit(size).ToList();
			}
			var builder = Builders<Entity>.Sort;
			List<SortDefinition<Entity>> sorts = new List<SortDefinition<Entity>>();
			foreach (var item in sort)
			{
				if (item.Value)
				{
					sorts.Add(builder.Ascending(item.Key));
				}
				else
				{
					sorts.Add(builder.Descending(item.Key));
				}
			}
			return await mongoCollection.Find(filter).Sort(Builders<Entity>.Sort.Combine(sorts)).Skip((index - 1) * size).Limit(size)?.ToListAsync();
		}
	}
}
