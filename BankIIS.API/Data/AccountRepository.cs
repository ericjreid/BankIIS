using BankIIS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankIIS.API.Data
{
	public class AccountRepository(BankDbContext context)
	{
		private BankDbContext db = context;

		public List<Account> Get()
		{
			return db.Accounts.ToList();
		}

		public Account Find(int? id)
		{
			return db.Accounts.Find(id);
		}

		public void Add(Account account) 
		{ 
			db.Accounts.Add(account);
			db.SaveChanges();
		}

		public void Update(Account account)
		{
			db.Entry(account).State = EntityState.Modified;
			db.SaveChanges();
		}

		public void Remove(Account account)
		{
			db.Accounts.Remove(account);
			db.SaveChanges();
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
