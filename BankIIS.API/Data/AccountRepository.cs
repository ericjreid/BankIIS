using BankIIS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Principal;

namespace BankIIS.API.Data
{
	public class AccountRepository(BankDbContext context)
	{
		private readonly BankDbContext _context = context;


		#region Custom Repo methods - deposit & withdraw
		public async Task<BalanceChange> Deposit(int accountId, decimal depositAmount, int customerId)
		{
			var accountUpdate = await _context.Accounts.FindAsync(accountId);
			if (accountUpdate is null)
				throw new ApplicationException("An error occurred finding the specified record.");

			if (accountUpdate.CustomerId != customerId)
				throw new ApplicationException("An error occurred between customer and account.");

			if (depositAmount <= 0)
				throw new ApplicationException("Deposit must exceed $0.");

			accountUpdate.Balance += depositAmount;
			await _context.SaveChangesAsync();

			BalanceChange deposit = new BalanceChange(customerId, accountId, depositAmount, true, accountUpdate.Balance);

			return deposit;
		}

		public async Task<BalanceChange> Withdraw(int accountId, decimal withdrawAmount, int customerId)
		{
			var accountUpdate = await _context.Accounts.FindAsync(accountId);
			if (accountUpdate is null)
				throw new ApplicationException("An error occurred finding the specified record.");

			if (accountUpdate.CustomerId != customerId)
				throw new ApplicationException("An error occurred between customer and account.");

			if (withdrawAmount <= 0)
				throw new ApplicationException("Withdrawal must exceed $0.");

			if (accountUpdate.Balance < withdrawAmount)
				throw new ApplicationException("Withdrawal cannot exceed account balance.");

			accountUpdate.Balance -= withdrawAmount;
			await _context.SaveChangesAsync();

			BalanceChange deposit = new BalanceChange(customerId, accountId, withdrawAmount, true, accountUpdate.Balance);

			return deposit;
		}
		#endregion


		#region Base CRUD Repo methods
		public async Task<List<Account>> Get()
		{
			try
			{
				return await _context.Accounts
				.Include(a => a.AccountType)
				.Include(a => a.Customer)
				.Include(a => a.Status)
				.ToListAsync();
			}
			catch (Exception)
			{
				throw new ApplicationException("An error occurred while fetching records.");
			}
		}

		public async Task<Account> Find(int id)
		{
			try
			{
				return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
			}
			catch (Exception)
			{
				throw new ApplicationException("An error occurred while fetching record details.");
			}
		}

		public async Task<Account> Add(Account newAccount)
		{
			try
			{
				_context.Accounts.Add(newAccount);
				await _context.SaveChangesAsync();

				return newAccount;
			}
			catch (Exception)
			{
				throw new ApplicationException("An error occurred while attempting to add record.");
			}
		}

		public async Task<Account> Update(int id, Account accountToUpdate)
		{
			var updatedAccount = await _context.Accounts.FindAsync(id);
			if (updatedAccount is null)
				throw new ApplicationException("An error occurred finding the specified record.");

			updatedAccount.AccountTypeId = accountToUpdate.AccountTypeId;
			updatedAccount.CustomerId = accountToUpdate.CustomerId;
			updatedAccount.StatusId = accountToUpdate.StatusId;
			updatedAccount.Balance = accountToUpdate.Balance;

			await _context.SaveChangesAsync();
			return updatedAccount;
		}

		public async Task Remove(int id)
		{
			try
			{
				var account = await _context.Accounts.FindAsync(id);
				if (account is null)
					throw new ApplicationException("An error occurred finding the specified record.");

				_context.Accounts.Remove(account);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw new ApplicationException("An error occurred while attempting to delete the record.");
			}
		}
		#endregion

		#region Dispose
		private bool disposed = false;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}
}
