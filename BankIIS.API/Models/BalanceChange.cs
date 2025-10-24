namespace BankIIS.API.Models
{
	/// <summary>
	/// Represents changes to account balance, either deposits or withdrawals.
	/// </summary>
	public class BalanceChange
	{
		public int CustomerId { get; set; }
		public int AccountId { get; set; }
		public decimal Amount { get; set; }
		public bool Succeeded { get; set; }
		public decimal Balance { get; set; }

		public BalanceChange(int customerId, int accountId, decimal amount, bool succeeded)
		{
			CustomerId = customerId;
			AccountId = accountId;
			Amount = amount;
			Succeeded = succeeded;
			Balance = 0;
		}

		public BalanceChange(int customerId, int accountId, decimal amount, bool succeeded, decimal balance)
		{
			CustomerId = customerId;
			AccountId = accountId;
			Amount = amount;
			Succeeded = succeeded;
			Balance = balance;
		}
	}
}
