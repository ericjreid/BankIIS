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
	}
}
