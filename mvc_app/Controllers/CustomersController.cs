using Microsoft.AspNetCore.Mvc;
using mvc_app.Models;
using mvc_app.Services;

namespace mvc_app.Controllers
{
    public class CustomersController : Controller
    {
        //private static List<Customer> _customers = new List<Customer>
        //{
        //    new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "doe@example.com" },
        //    new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "smith@example.com" },
        //    new Customer { Id = 3, FirstName = "Alice", LastName = "Brown", Email = "brown@example.com" }
        //};
        //private static int _nextId = 4;
        private readonly IServiceCustomers? _serviceCustomers;
        private readonly CustomerContext? _customerContext;

        public CustomersController(IServiceCustomers serviceCustomers, CustomerContext customerContext)
        {
            _serviceCustomers = serviceCustomers;
            _customerContext = customerContext;
            _serviceCustomers._customerContext = _customerContext;
        }

        public IActionResult Index() => View(_serviceCustomers?.Index());

        public IActionResult Details(int id) => View(_serviceCustomers?.Details(id));

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FirstName,LastName,Email")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _ = _serviceCustomers?.Create(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Edit(int id)
        {
            var customer = _customerContext?.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,FirstName,LastName,Email")] Customer customer, int id)
        {
            if (ModelState.IsValid)
            {
                _ = _serviceCustomers?.Edit(id, customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Delete(int id)
        {
            var customer = _customerContext?.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _serviceCustomers?.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
