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
    public class APICustomerController : ControllerBase
    {
        private readonly IServiceCustomers _serviceCustomers;

        public APICustomerController(IServiceCustomers serviceCustomers)
        {
            _serviceCustomers = serviceCustomers;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _serviceCustomers.IndexAsync();
            return Ok(customers);
        }

        // Получение поста по Id (доступно всем)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _serviceCustomers.ReadAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        // Создание нового поста (доступно только авторизованным пользователям)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            var newCustomer = await _serviceCustomers.CreateAsync(customer);
            return Ok(newCustomer);
        }

        // Обновление поста (доступно только авторизованным пользователям)
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer([Bind("Id,FirstName,LastName,Email")] int id, Customer customer)
        {
            var updatedCustomer = await _serviceCustomers.UpdateAsync(customer.Id, customer);
            return Ok(updatedCustomer);
        }

        // Удаление поста (доступно только авторизованным пользователям)
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _serviceCustomers.DeleteAsync(id);
            return Ok(new { message = result });
        }
    }
}
