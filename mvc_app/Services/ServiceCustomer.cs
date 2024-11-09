using Microsoft.EntityFrameworkCore;
using mvc_app.Models;

namespace mvc_app.Services
{
    public interface IServiceCustomers
    {
        public CustomerContext? _customerContext { get; set; }
        public IEnumerable<Customer> Index();
        public Customer Create(Customer customer);
        public Customer Details(int id);
        public Customer Edit(int id, Customer customer);
        public bool Delete(int id);
    }
    public class ServiceCustomer : IServiceCustomers
    {
        public CustomerContext? _customerContext { get; set; }

        public Customer Create(Customer customer)
        {
            _customerContext?.Customers.Add(customer);
            _customerContext?.SaveChanges();
            return customer;
        }

        public bool Delete(int id)
        {
            var customer = _customerContext?.Customers.Find(id);
            if (customer == null) return false;
            _customerContext?.Customers.Remove(customer);
            _customerContext?.SaveChanges();
            return true;
        }

        public Customer? Details(int id)
        {
            Customer? customer = _customerContext?.Customers
                .FirstOrDefault(c => c.Id == id);
            return customer;
        }

        public Customer Edit(int id, Customer customer)
        {
            if (id != customer.Id) return null;
            else
            {
                try
                {
                    _customerContext?.Customers.Update(customer);
                    _customerContext?.SaveChanges();
                    return customer;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return null;
                }
            }
        }

        public Customer Edit(Customer customer)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer>? Index()
        {
            return _customerContext?.Customers.ToList();
        }
    }
}
