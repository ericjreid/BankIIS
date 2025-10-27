using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BankIIS.API.Data
{
	public class GenericRepository<TEntity> where TEntity : class, IGenericRepository<TEntity>
	{
		private BankDbContext db = new BankDbContext(null);

		//public virtual List<TEntity> Get()
		//{
		//	return db.Set<TEntity>().ToList();
		//}

		//public virtual async IAsyncEnumerable<TEntity> Get()
		//{
		//	yield return db.Set<TEntity>().ToList();
		//}


		//public TEntity Find(object id) 
		//{
		//	return db.Set<TEntity>().Find(id);
		//}

		//public async Task<TEntity> Find(object id)
		//{
		//	try
		//	{
		//		return await db.Set<TEntity>().FirstOrDefaultAsync(te => te.Id == id);
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new ApplicationException("An error occurred while fetching record details.");
		//	}


		//	return db.Set<TEntity>().Find(id);
		//}

		public void Add(TEntity entity)
		{
			db.Set<TEntity>().Add(entity);
			db.SaveChanges();
		}

		public void Update(TEntity entity)
		{
			db.Entry(entity).State = EntityState.Modified;
			db.SaveChanges();
		}

		public void Remove(TEntity entity)
		{
			db.Set<TEntity>().Remove(entity);
			db.SaveChanges();
		}

		public void Remove(object id)
		{
			var entity = db.Set<TEntity>().Find(id);
			Remove(entity);
		}

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					db.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

	}
}
