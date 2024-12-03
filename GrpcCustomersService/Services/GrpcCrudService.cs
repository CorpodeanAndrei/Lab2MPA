using Grpc.Core;
using GrpcCustomersService;
using DataAccess = LibraryModel.Data;
using ModelAccess = LibraryModel.Models;

namespace GrpcCustomersService.Services
{
    public class GrpcCrudService : CustomerService.CustomerServiceBase
    {

        private DataAccess.Lab2MPAContext db = null;
        public GrpcCrudService(DataAccess.Lab2MPAContext db)
        {
            this.db = db;
        }

        public override Task<CustomerList> GetAll(Empty empty, ServerCallContext context)
        {
            CustomerList pl = new CustomerList();
            var query = from cust in db.Customer
                        select new Customer()
                        {
                            CustomerId = cust.CustomerID,
                            Name = cust.Name,
                            Adress = cust.Adress
                        };
            pl.Item.AddRange(query.ToArray());
            return Task.FromResult(pl);
        }


        //public override Task<Empty> Insert(Customer requestData, ServerCallContext context)
        //{
        //    db.Customer.Add(new ModelAccess.Customer
        //    {
        //        CustomerID = requestData.CustomerId,
        //        Name = requestData.Name,
        //        Adress = requestData.Adress,
        //        BirthDate = DateTime.Parse(requestData.Birthdate)
        //    });
        //    db.SaveChanges();
        //    return Task.FromResult(new Empty());
        //}

        public override Task<Empty> Insert(Customer requestData, ServerCallContext context)
        {
            //try
            //{
                //var cityId = requestData.CityID == 0 ? 1 : requestData.CityID;
            Console.WriteLine($"Customer received: ID={requestData.CustomerId}, Name={requestData.Name}, Adress={requestData.Adress}, BirthDate={requestData.Birthdate}");

                // Map gRPC Customer to internal Customer model
                var customer = new LibraryModel.Models.Customer
                {
                    CustomerID = requestData.CustomerId,
                    Name = requestData.Name,
                    Adress = requestData.Adress,
                    BirthDate = !string.IsNullOrWhiteSpace(requestData.Birthdate)
                        ? DateTime.Parse(requestData.Birthdate)
                        : (DateTime?)null,
                    CityID = 1
                };

                // Save to database
                db.Customer.Add(customer);
                db.SaveChanges();

                return Task.FromResult(new Empty());
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Eroare inserare: {ex.Message}");
            //    throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
            //}
        }

        public override Task<Customer> Get(CustomerId request, ServerCallContext context)
        {
            var customer = db.Customer
                .Where(c => c.CustomerID == request.Id)
                .Select(c => new Customer
                {
                    CustomerId = c.CustomerID,
                    Name = c.Name,
                    Adress = c.Adress,
                    Birthdate = c.BirthDate.HasValue ? c.BirthDate.Value.ToString("yyyy-MM-dd") : null
                })
                .FirstOrDefault();

            if (customer == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            }

            return Task.FromResult(customer);
        }

        public override Task<Customer> Update(Customer request, ServerCallContext context)
        {
            var customer = db.Customer.FirstOrDefault(c => c.CustomerID == request.CustomerId);

            if (customer == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            }

            customer.Name = request.Name;
            customer.Adress = request.Adress;
            customer.BirthDate = !string.IsNullOrWhiteSpace(request.Birthdate)
                ? DateTime.Parse(request.Birthdate)
                : null;

            db.SaveChanges();
            return Task.FromResult(request);
        }

        public override Task<Empty> Delete(CustomerId request, ServerCallContext context)
        {
            var customer = db.Customer.FirstOrDefault(c => c.CustomerID == request.Id);

            if (customer == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            }

            db.Customer.Remove(customer);
            db.SaveChanges();
            return Task.FromResult(new Empty());
        }


    }

}