using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MeetOct.MongoExtensions
{
	public interface IMongoRepository<Entity>
	{

		void InsertOne(Entity entity);

		void InsertList(List<Entity> entity);

		long DeleteOne(Expression<Func<Entity, bool>> where);

		long DeleteList(Expression<Func<Entity, bool>> where);

		void UpdateOne(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update);

		void UpdateList(Expression<Func<Entity, bool>> where, Dictionary<string, dynamic> update);


		Entity FindFirstOne(Expression<Func<Entity, bool>> where);

		List<Entity> FindList(Expression<Func<Entity, bool>> where);

		List<Entity> PageList(int index, int size, Expression<Func<Entity, bool>> where);

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="index"></param>
		/// <param name="size"></param>
		/// <param name="where"></param>
		/// <param name="sort">排序方式，key代表排序字段，value 代表是否asc</param>
		/// <returns></returns>
		List<Entity> PageList(int index, int size, Expression<Func<Entity, bool>> where, Dictionary<string, bool> sort = null);
	}
}
