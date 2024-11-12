using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvc_app.Models;
using mvc_app.Services;

namespace mvc_app.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IServiceCustomers _serviceCustomers;

        public CustomersController(IServiceCustomers serviceCustomers)
        {
            _serviceCustomers = serviceCustomers;
        }
        [HttpGet]
        public async Task<ViewResult> Index()
        {
            return View(await _serviceCustomers.IndexAsync());
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _serviceCustomers.CreateAsync(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        [HttpGet]
        public async Task<ViewResult> Read(int id)
        {
            return View(await _serviceCustomers.ReadAsync(id));
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ViewResult> Update(int id)
        {
            return View(await _serviceCustomers.ReadAsync(id));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Update([Bind("Id,FirstName,LastName,Email")] Customer customer, int id)
        {
            if (ModelState.IsValid)
            {
                await _serviceCustomers.UpdateAsync(id, customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ViewResult> Delete(int id)
        {
            var customer = await _serviceCustomers.ReadAsync(id);
            if (customer == null)
            {
                return View("NotFound");
            }
            return View(customer);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceCustomers.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
