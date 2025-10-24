using Microsoft.EntityFrameworkCore;

namespace BankIIS.API.Data
{
	public class GenericRepository<TEntity>(BankDbContext context) where TEntity : class
	{
		private BankDbContext db = context;

		public virtual List<TEntity> Get()
		{
			return db.Set<TEntity>().ToList();
		}

		public TEntity Find(object id) 
		{
			return db.Set<TEntity>().Find(id);
		}

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
