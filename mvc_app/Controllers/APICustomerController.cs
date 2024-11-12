using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using mvc_app.Models;
using mvc_app.Services;
using System.Security.Claims;

namespace mvc_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APICustomerController : ControllerBase
    {
        private readonly IServiceCustomers _serviceCustomers;
        private readonly ILogger<APICustomerController> _logger;

        public APICustomerController(IServiceCustomers serviceCustomers, ILogger<APICustomerController> logger)
        {
            _serviceCustomers = serviceCustomers;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            _logger.LogInformation("Fetching all customers.");
            var customers = await _serviceCustomers.IndexAsync();
            return Ok(customers);
        }

        // Получение поста по Id (доступно всем)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            _logger.LogInformation($"Fetching customer with ID: {id}");
            var customer = await _serviceCustomers.ReadAsync(id);
            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found.");
                return NotFound();
            }
            return Ok(customer);
        }

        // Создание нового поста (доступно только авторизованным пользователям)
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                _logger.LogWarning("Attempted to create a null customer.");
                return BadRequest("Customer object is null.");
            }

            _logger.LogInformation("Creating a new customer.");
            var newCustomer = await _serviceCustomers.CreateAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
        }

        // Обновление поста (доступно только авторизованным пользователям)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer([Bind("Id,FirstName,LastName,Email")] int id, Customer customer)
        {
            if (customer == null)
            {
                _logger.LogError("UpdateCustomer: Product object is null.");
                return BadRequest("Customer object is null.");
            }

            _logger.LogInformation($"Updating customer with ID: {id}");
            var productUpdated = await _serviceCustomers.UpdateAsync(id, customer);
            if (productUpdated == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found for update.");
                return NotFound();
            }
            return Ok(productUpdated);
        }

        // Удаление поста (доступно только авторизованным пользователям)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation($"Deleting customer with ID: {id}");
            var deleted = await _serviceCustomers.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning($"Customer with ID {id} not found for deletion.");
                return NotFound();
            }
            return Ok(new { message = "Customer deleted successfully." });
        }
    }
}
