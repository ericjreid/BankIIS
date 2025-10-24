using BankIIS.API.Data;
using BankIIS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankIIS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController(BankDbContext context) : ControllerBase
	{
		private readonly BankDbContext _context = context;

		[HttpGet]
		public async Task<ActionResult<List<Customer>>> GetCustomers()
		{
			return Ok(await _context.Customers.ToListAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Customer>> GetCustomerById(int id)
		{
			var customer = await _context.Customers.FindAsync(id);
			if (customer is null)
				return NotFound();

			return Ok(customer);
		}

		[HttpPost]
		public async Task<ActionResult<Customer>> AddCustomer(Customer newCustomer)
		{
			if (newCustomer is null)
				return BadRequest();

			_context.Customers.Add(newCustomer);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
		{
			var customer = await _context.Customers.FindAsync(id);
			if (customer is null)
				return NotFound();

			customer.FirstName = updatedCustomer.FirstName;
			customer.LastName = updatedCustomer.LastName;

			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCustomer(int id)
		{
			var customer = await _context.Customers.FindAsync(id);
			if (customer is null)
				return NotFound();

			_context.Customers.Remove(customer);
			await _context.SaveChangesAsync();
			return NoContent();
		}



		#region v1 static list approach
		//static private List<Customer> customers = new List<Customer>
		//{
		//	new Customer { Id = 1, FirstName = "Carole", LastName = "Burnett" },
		//	new Customer { Id = 2, FirstName = "Jack", LastName = "Black"},
		//	new Customer { Id = 3, FirstName = "Robin", LastName = "Williams"}
		//};


		//[HttpGet]
		//public ActionResult<List<Customer>> GetCustomers()
		//{
		//	return Ok(customers);
		//}

		//[HttpGet("{id}")]
		//public ActionResult<Customer> GetCustomerById(int id)
		//{
		//	var customer = customers.FirstOrDefault(c => c.Id == id);
		//	if (customer is null)
		//		return NotFound();

		//	return Ok(customer);
		//}

		//[HttpPost]
		//public ActionResult<Customer> AddCustomer(Customer newCustomer)
		//{
		//	if (newCustomer is null)
		//		return BadRequest();

		//	newCustomer.Id = customers.Max(c => c.Id) + 1;
		//	customers.Add(newCustomer);
		//	return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
		//}

		//[HttpPut("{id}")]
		//public IActionResult UpdateCustomer(int id, Customer updatedCustomer)
		//{
		//	var customer = customers.FirstOrDefault(c => c.Id == id);
		//	if (customer is null)
		//		return NotFound();

		//	customer.FirstName = updatedCustomer.FirstName;
		//	customer.LastName = updatedCustomer.LastName;

		//	return NoContent();
		//}

		//[HttpDelete("{id}")]
		//public IActionResult DeleteCustomer(int id)
		//{
		//	var customer = customers.FirstOrDefault(c => c.Id == id);
		//	if (customer is null)
		//		return NotFound();

		//	customers.Remove(customer);
		//	return NoContent();
		//}
		#endregion
	}
}
