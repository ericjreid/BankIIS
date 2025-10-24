using System.ComponentModel.DataAnnotations.Schema;

namespace BankIIS.API.Models
{
	public class Account
	{
		public int Id { get; set; }

		public int AccountTypeId { get; set; }
		public AccountType? AccountType { get; set; }

		public int CustomerId { get; set; }
		public Customer? Customer { get; set; }

		public int StatusId { get; set; }
		public Status? Status { get; set; }

		[Column(TypeName = "money")]
		public decimal Balance { get; set; }
	}
}
