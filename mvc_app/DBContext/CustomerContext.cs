using Microsoft.EntityFrameworkCore;
using mvc_app.Models;

namespace mvc_app
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }
    }
}
