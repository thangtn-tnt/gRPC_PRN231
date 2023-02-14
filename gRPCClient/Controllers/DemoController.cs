using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using OwnProtos;
using static OwnProtos.GrpcCustomer;

namespace gRPCClient.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcCustomerClient(channel);

            CustomerList data = client.GetAll(new OwnProtos.Empty());
            ViewBag.Data = data;
            return View();
        }

        public IActionResult Get(string id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcCustomerClient(channel);

            Customer cus = client.GetCustomer(new IDRequest() { Id = id });
            ViewBag.Customer = cus;
            return View();
        }
    }
}