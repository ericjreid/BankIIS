using BankIIS.API.Data;
using BankIIS.API.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankIIS.TEST.Repos
{
	public class AccountRepoTests
	{
		#region InMemory DB mock-up
		private async Task<BankDbContext> GetDbContext()
		{
			var options = new DbContextOptionsBuilder<BankDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var databaseContext = new BankDbContext(options);
			databaseContext.Database.EnsureCreated();
			if (await databaseContext.Accounts.CountAsync() <= 0)
			{
				for(int i = 0; i < 10; i++)
				{
					databaseContext.Accounts.Add(
						new Account { Id=i+1, AccountTypeId = 2, CustomerId = 1, StatusId = 1, Balance = 1000 }
						);
					await databaseContext.SaveChangesAsync();
				}
			}
			return databaseContext;			
		}
		#endregion

		#region sample crud tests
		[Fact]
		public async void AccountRepo_Find_ReturnsTaskAccount()
		{
			//Arrange
			int id = 1;
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			var result = accountRepo.Find(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<Task<Account>>();
		}

		[Fact]
		public async void AccountRepo_Get_ReturnsTaskListAccountOfTen()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			var result = accountRepo.Get();

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<Task<List<Account>>>();
		}
		#endregion

		#region Deposit tests
		[Fact]
		public async void AccountRepo_Deposit_UpdatesBalanceBySetAmount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			var begBalance = accountRepo.Find(1).Result.Balance;
			var account = accountRepo.Find(1).Result;
			var result = accountRepo.Deposit(1, 100, 1).Result;


			//Assert
			result.Should().NotBeNull();
			result.Balance.Should().Be(begBalance + 100);
		}


		[Fact]
		public async void AccountRepo_Deposit_FailsOnIncorrectAccount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Deposit(99, 100, 1).Result;//99 account ID doesn't exist
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}


		[Fact]
		public async void AccountRepo_Deposit_FailsOnIncorrectCustomer()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Deposit(1, 100, 99).Result;//99 is incorrect customerId
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}

		[Fact]
		public async void AccountRepo_Deposit_FailsOnZeroAmount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Deposit(1, 0, 1).Result;//0 is disallowed deposit amount
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}

		[Fact]
		public async void AccountRepo_Deposit_FailsOnNegativeAmount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Deposit(1, -99, 1).Result;//negative numbers are disallowed deposit amount
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}
		#endregion

		#region Withdraw tests
		[Fact]
		public async void AccountRepo_Withdraw_UpdatesBalanceBySetAmount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			var begBalance = accountRepo.Find(1).Result.Balance;
			var account = accountRepo.Find(1).Result;
			var result = accountRepo.Withdraw(1, 100, 1).Result;


			//Assert
			result.Should().NotBeNull();
			result.Balance.Should().Be(begBalance - 100);
		}


		[Fact]
		public async void AccountRepo_Withdraw_FailsOnIncorrectAccount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Withdraw(99, 100, 1).Result;//99 account ID doesn't exist
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}


		[Fact]
		public async void AccountRepo_Withdraw_FailsOnIncorrectCustomer()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Withdraw(1, 100, 99).Result;//99 is incorrect customerId
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}

		[Fact]
		public async void AccountRepo_Withdraw_FailsOnZeroAmount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Withdraw(1, 0, 1).Result;//0 is disallowed withdraw amount
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}

		[Fact]
		public async void AccountRepo_Withdraw_FailsOnNegativeAmount()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Withdraw(1, -99, 1).Result;//negative numbers are disallowed withdraw amount
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}


		[Fact]
		public async void AccountRepo_Withdraw_FailsWhenExceedsBalance()
		{
			//Arrange
			var dbContext = await GetDbContext();
			var accountRepo = new AccountRepository(dbContext);

			//Act
			BalanceChange result = null;
			try
			{
				result = accountRepo.Withdraw(1, 9999, 1).Result;//withdraw amount that exceeds balance is disallowed
			}
			catch (Exception)
			{
				//do nothing, allowing result to remain null
			}

			//Assert
			result.Should().BeNull();
		}
		#endregion

	}
}
