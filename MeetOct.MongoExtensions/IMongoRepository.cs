using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MeetOct.MongoExtensions
{
	public interface IMongoRepository<Entity>
	{

		Task InsertOneAsync(Entity entity);

        Task InsertListAsync(List<Entity> entity);

        Task<long> DeleteOneAsync(Expression<Func<Entity, bool>> where);

		Task<long> DeleteListAsync(Expression<Func<Entity, bool>> where);

        Task UpdateOneAsync(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update);

        Task UpdateListAsync(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update);

        Task<Entity> FindOneAndUpdateAsync(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update);

        Task<Entity> FindFirstOneAsync(Expression<Func<Entity, bool>> where);

		Task<List<Entity>> FindListAsync(Expression<Func<Entity, bool>> where);

		Task<List<Entity>> PageListAsync(int index, int size, Expression<Func<Entity, bool>> where);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="where"></param>
        /// <param name="sort">排序方式，key代表排序字段，value 代表是否asc</param>
        /// <returns></returns>
        Task<List<Entity>> PageListAsync(int index, int size, Expression<Func<Entity, bool>> where, Dictionary<string, bool> sort = null);
	}
}
