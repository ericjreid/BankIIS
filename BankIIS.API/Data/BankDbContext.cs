using BankIIS.API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BankIIS.API.Data
{
	public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
	{
		public DbSet<Customer> Customers => Set<Customer>();
		public DbSet<Account> Accounts => Set<Account>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Customer>().HasData(
				new Customer { Id = 1, FirstName = "Carole", LastName = "Burnett" },
				new Customer { Id = 2, FirstName = "Jack", LastName = "Black" },
				new Customer { Id = 3, FirstName = "Robin", LastName = "Williams" }
			);

			modelBuilder.Entity<AccountType>().HasData(
				new AccountType { Id = 1, Type = "Checking" },
				new AccountType { Id = 2, Type = "Savings" }
				);

			modelBuilder.Entity<Status>().HasData(
				new Status { Id = 1, StatusName = "Open" },
				new Status { Id = 2, StatusName = "Closed" }
				);

			modelBuilder.Entity<Account>().HasData(
				new Account { Id = 1, AccountTypeId = 2, CustomerId = 1, StatusId = 1, Balance = 1000 },
				new Account { Id = 2, AccountTypeId = 1, CustomerId = 1, StatusId = 1, Balance = 20000 },
				new Account { Id = 3, AccountTypeId = 2, CustomerId = 2, StatusId = 1, Balance = 10000 },
				new Account { Id = 4, AccountTypeId = 1, CustomerId = 2, StatusId = 2, Balance = 0 }
				);
		}
	}
}
