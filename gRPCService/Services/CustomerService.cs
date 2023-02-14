using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using gRPCService.Models;
using OwnProtos;
using static OwnProtos.GrpcCustomer;

namespace gRPCService.Services
{
    public class CustomerService : GrpcCustomerBase
    {
        public readonly gRPCContext _db;

        public CustomerService(gRPCContext db)
        {
            _db = db;
        }

        public override Task<CustomerList> GetAll(Empty requestData, ServerCallContext context)
        {
            CustomerList response = new CustomerList();

            var cusList = from o in this._db.Customers
                          select new OwnProtos.Customer()
                          {
                              Id = o.Id,
                              Name = o.Name,
                              Address = o.Address
                          };

            response.Customers.AddRange(cusList);
            return Task.FromResult(response);
        }

        public override Task<OwnProtos.Customer> GetCustomer(IDRequest request, ServerCallContext context)
        {
            var objFromDb = this._db.Customers.Find(request.Id);

            OwnProtos.Customer cus = new()
            {
                Id = objFromDb.Id,
                Name = objFromDb.Name,
                Address = objFromDb.Address
            };

            return Task.FromResult(cus);
        }
    }
}