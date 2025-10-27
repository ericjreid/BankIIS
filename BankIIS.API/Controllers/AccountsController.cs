using BankIIS.API.Data;
using BankIIS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankIIS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController(BankDbContext context) : ControllerBase
	{
		private readonly BankDbContext _context = context;//TODO: ejr - REMOVE along with v1 not-repo approaches when repo approach fixed
		private readonly AccountRepository _repo = new AccountRepository(context);


		#region Repo async custom actions - deposit & withdrawal
		[HttpPut("deposit/{accountId}")]
		public async Task<IActionResult> Deposit(int accountId, decimal depositAmount, int customerId)
		{
			var depositDetails = await _repo.Deposit(accountId, depositAmount, customerId);
			return Ok(depositDetails);
		}

		[HttpPut("withdraw/{accountId}")]
		public async Task<IActionResult> Withdraw(int accountId, decimal withdrawAmount, int customerId)
		{
			var withdrawDetails = await _repo.Withdraw(accountId, withdrawAmount, customerId);
			return Ok(withdrawDetails);
		}

		#endregion


		#region Repo async CRUD Actions
		[HttpGet(nameof(Get))]
		public async Task<ActionResult<List<Account>>> Get()
		{
			try
			{
				return Ok(await _repo.Get());
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpGet(nameof(Find))]
		public async Task<IActionResult> Find(int id)
		{
			try
			{
				var account = await _repo.Find(id);

				if (account == null)
					return NotFound();

				return Ok(account);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpPost(nameof(Add))]
		public async Task<IActionResult> Add(Account newAccount)
		{
			try
			{
				if (newAccount is null)
					return BadRequest();

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var createdAccount = await _repo.Add(newAccount);

				return CreatedAtAction(nameof(Find), new { id = createdAccount.Id }, createdAccount);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal Server Error: {ex.Message}");
			}
		}

		[HttpPut("Update/{id}")]
		public async Task<IActionResult> Update(int id, Account accountToUpdate)
		{
			var updatedAccount = await _repo.Update(id, accountToUpdate);
			return Ok(updatedAccount);
		}

		[HttpDelete("Remove/{id}")]
		public async Task<IActionResult> Remove(int id)
		{
			await _repo.Remove(id);
			return Ok("Account deleted successfully.");
		}
		#endregion


		#region Base async custom actions - deposit & withdrawal
		[HttpPut("{accountId}/deposit")]
		public async Task<IActionResult> MakeDeposit(int accountId, decimal depositAmount, int customerId)
		{
			var account = await _context.Accounts.FindAsync(accountId);
			if (account is null)
				return NotFound();

			if (account.CustomerId != customerId)
				return NotFound();

			if (depositAmount <= 0)
				return BadRequest();

			account.Balance += depositAmount;

			await _context.SaveChangesAsync();

			BalanceChange deposit = new BalanceChange(customerId, accountId, depositAmount, true, account.Balance);

			return Ok(deposit);
		}

		[HttpPut("{accountId}/withdraw")]
		public async Task<IActionResult> MakeWithdrawal(int accountId, decimal withdrawAmount, int customerId)
		{
			var account = await _context.Accounts.FindAsync(accountId);
			if (account is null)
				return NotFound();

			if (account.CustomerId != customerId)
				return NotFound();

			if (withdrawAmount <= 0)
				return BadRequest();

			if (account.Balance < withdrawAmount)
				return BadRequest();

			account.Balance -= withdrawAmount;

			await _context.SaveChangesAsync();

			BalanceChange withdrawal = new BalanceChange(customerId, accountId, withdrawAmount, true, account.Balance);

			return Ok(withdrawal);
		}
		#endregion

		#region Base Async CRUD Actions
		[HttpGet]
		public async Task<ActionResult<List<Account>>> GetAccounts()
		{
			return Ok(await _context.Accounts
				.Include(a => a.AccountType)
				.Include(a => a.Customer)
				.Include(a => a.Status)
				.ToListAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Account>> GetAccountById(int id)
		{
			var account = await _context.Accounts.FindAsync(id);
			if (account is null)
				return NotFound();

			return Ok(account);
		}

		[HttpPost]
		public async Task<ActionResult<Account>> AddAccount(Account newAccount)
		{
			if (newAccount is null)
				return BadRequest();

			_context.Accounts.Add(newAccount);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAccountById), new { id = newAccount.Id }, newAccount);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAccount(int id, Account updatedAccount)
		{
			var account = await _context.Accounts.FindAsync(id);
			if (account is null)
				return NotFound();

			account.AccountTypeId = updatedAccount.AccountTypeId;
			account.CustomerId = updatedAccount.CustomerId;
			account.StatusId = updatedAccount.StatusId;
			account.Balance = updatedAccount.Balance;

			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAccount(int id)
		{
			var account = await _context.Accounts.FindAsync(id);
			if (account is null)
				return NotFound();

			_context.Accounts.Remove(account);
			await _context.SaveChangesAsync();
			return NoContent();
		}
		#endregion
	}
}
