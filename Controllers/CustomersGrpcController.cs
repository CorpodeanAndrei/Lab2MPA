using Grpc.Core;
using Grpc.Net.Client;
using GrpcCustomersService;
using Lab2MPA.Models;
using LibraryModel.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Customer = GrpcCustomersService.Customer;


namespace Lab2MPA.Controllers
{
    public class CustomersGrpcController : Controller
    {
        private readonly GrpcChannel channel;
        public CustomersGrpcController()
        {
            channel = GrpcChannel.ForAddress("https://localhost:7219");
        }
        [HttpGet]
        public IActionResult Index()
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            CustomerList cust = client.GetAll(new Empty());
            return View(cust);
        }
        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Create(Customer customer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var client = new CustomerService.CustomerServiceClient(channel);
        //        var createdCustomer = client.Insert(customer);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(customer);
        //}

        [HttpPost]
        public IActionResult Create(LibraryModel.Models.Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new CustomerService.CustomerServiceClient(channel);

                    // Map internal Customer to gRPC Customer
                    var grpcCustomer = new GrpcCustomersService.Customer
                    {
                        CustomerId = customer.CustomerID,
                        Name = customer.Name ?? string.Empty,
                        Adress = customer.Adress ?? string.Empty,
                        Birthdate = customer.BirthDate.HasValue
                            ? customer.BirthDate.Value.ToString("yyyy-MM-dd")
                            : string.Empty,
                        CityId = 1
                    };

                    // Call gRPC Insert method
                    client.Insert(grpcCustomer);

                    return RedirectToAction(nameof(Index));
                }
                catch (RpcException ex)
                {
                    ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                }

            }

            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            var customer = client.Get(new CustomerId { Id = id });
            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = new CustomerService.CustomerServiceClient(channel);
                client.Update(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            var customer = client.Get(new CustomerId { Id = id });
            return View(customer);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            var customer = client.Get(new CustomerId { Id = id });
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            client.Delete(new CustomerId { Id = id });
            return RedirectToAction(nameof(Index));
        }

    }
}