using BankIIS.API.Models;

namespace BankIIS.API.Data
{
	public interface IGenericRepository<TEntity> where TEntity : class
	{
		public Task<TEntity> Get();
		public Task<TEntity> Find(object id);
		public Task Add(TEntity entity);
		public Task Update(TEntity entity);
		public Task Remove(TEntity entity);
		public Task Remove(object id);
		public Task Foo();//TODO: ejr - test enforced, then remove
	}
}
