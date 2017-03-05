using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

		public void InsertOne(Entity entity)
		{
			mongoCollection.InsertOne(entity);
		}

		public void InsertList(List<Entity> entity)
		{
			mongoCollection.InsertMany(entity);
		}

		public long DeleteOne(Expression<Func<Entity, bool>> where)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			return mongoCollection.DeleteOne(filter).DeletedCount;
		}

		public long DeleteList(Expression<Func<Entity, bool>> where)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			return mongoCollection.DeleteMany(filter).DeletedCount;
		}

		public void UpdateOne(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			var builder = Builders<Entity>.Update;
			List<UpdateDefinition<Entity>> updates = new List<UpdateDefinition<Entity>>();
			foreach (var item in update)
			{
				updates.Add(builder.Set(item.Key, item.Value));
			}
			mongoCollection.UpdateOne(filter, Builders<Entity>.Update.Combine(updates));
		}

		public void UpdateList(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update)
		{
			var filter = Builders<Entity>.Filter.Where(where);
			var builder = Builders<Entity>.Update;
			List<UpdateDefinition<Entity>> updates = new List<UpdateDefinition<Entity>>();
			foreach (var item in update)
			{
				updates.Add(builder.Set(item.Key, item.Value));
			}
			mongoCollection.UpdateMany(filter, Builders<Entity>.Update.Combine(updates));
		}


		public Entity FindFirstOne(Expression<Func<Entity, bool>> where)
		{
			return mongoCollection.Find(Builders<Entity>.Filter.Where(where)).FirstOrDefault();
		}

		public List<Entity> FindList(Expression<Func<Entity, bool>> where)
		{
			return mongoCollection.Find(Builders<Entity>.Filter.Where(where)).ToList();
		}

		public List<Entity> PageList(int index, int size, Expression<Func<Entity, bool>> where)
		{
			return PageList(index, size, where, null);
		}

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="index"></param>
		/// <param name="size"></param>
		/// <param name="where"></param>
		/// <param name="sort">排序方式，key代表排序字段，value 代表是否asc</param>
		/// <returns></returns>
		public List<Entity> PageList(int index, int size, Expression<Func<Entity, bool>> where, Dictionary<string, bool> sort = null)
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
			return mongoCollection.Find(filter).Sort(Builders<Entity>.Sort.Combine(sorts)).Skip((index - 1) * size).Limit(size).ToList();
		}
	}
}
