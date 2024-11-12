using Microsoft.EntityFrameworkCore;
using mvc_app.Models;

namespace mvc_app.Services
{
    public interface IServiceCustomers
    {
        public Task<IEnumerable<Customer>> IndexAsync();
        public Task<Customer?> CreateAsync(Customer? customer);
        public Task<Customer?> ReadAsync(int id);
        public Task<Customer?> UpdateAsync(int id, Customer? customer);
        public Task<bool> DeleteAsync(int id);
    }
    public class ServiceCustomer : IServiceCustomers
    {
        private readonly CustomerContext? _customerContext;
        private readonly ILogger<ServiceCustomer>? _logger;
        public ServiceCustomer(CustomerContext? customerContext, ILogger<ServiceCustomer>? logger)
        {
            _customerContext = customerContext;
            _logger = logger;
        }

        async Task<IEnumerable<Customer>> IServiceCustomers.IndexAsync()
        {
            return await _customerContext.Customers.ToListAsync();
        }

        public async Task<Customer?> CreateAsync(Customer? customer)
        {
            if (customer == null)
            {
                _logger?.LogWarning("Attempted to create a null customer");
                return null;
            }
            await _customerContext.Customers.AddAsync(customer);
            await _customerContext.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> ReadAsync(int id)
        {
            return await _customerContext.Customers.FindAsync(id);
        }

        public async Task<Customer?> UpdateAsync(int id, Customer? customer)
        {
            if (id != customer.Id || id == null)
            {
                _logger.LogWarning("Attempted to update a customer with mismatched ID");
                return null;
            }
            try
            {
                _customerContext.Customers.Update(customer);
                await _customerContext.SaveChangesAsync();
                return customer;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _customerContext.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger?.LogWarning("Attempted to delete a non-existent customer");
                return false;
            }
            _customerContext.Customers.Remove(customer);
            await _customerContext.SaveChangesAsync();
            return true;
        }
    }
}
