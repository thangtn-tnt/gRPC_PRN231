using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gRPC.Protos;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using static gRPC.Protos.GrpcProduct;
using static gRPC.Protos.GrpcCategory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gRPCClient.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcProductClient(channel);

            ProductList data = client.GetAll(new Empty());
            ViewBag.Data = data;
            return View();
        }

        public IActionResult Create()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcCategoryClient(channel);

            CategoryList data = client.GetAll(new EmptyCate());
            ViewData["CategoryName"] = new SelectList(data.Categories, "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new GrpcProductClient(channel);

                TempData["message"] = client.CreateProduct(product).Message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }
        public IActionResult Delete(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcProductClient(channel);            

            ProductResponse data = client.GetProduct(new ProductIDRequest() { Id = id });

            if (data.Id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Product product)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcProductClient(channel);

            TempData["message"] = client.DeleteProduct(new ProductIDRequest() { Id = product.Id }).Message;
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var cateClient = new GrpcCategoryClient(channel);
            var proClient = new GrpcProductClient(channel);

            CategoryList data = cateClient.GetAll(new EmptyCate());

            ProductResponse proData = proClient.GetProduct(new ProductIDRequest() { Id = id });

            if (proData.Id == 0)
            {
                return RedirectToAction("Index");
            }

            Product product = new Product()
            {
                Id = proData.Id,
                ProductName = proData.ProductName,
                Price = proData.Price,
                CategoryId = data.Categories.Where(x => x.CategoryName.Equals(proData.CategoryName)).FirstOrDefault().Id
            };

            ViewData["CategoryName"] = new SelectList(data.Categories, "Id", "CategoryName");
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Product product)
        {
            if (ModelState.IsValid)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new GrpcProductClient(channel);

                TempData["message"] = client.UpdateProduct(product).Message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }

        public IActionResult Details(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new GrpcProductClient(channel);

            ProductResponse data = client.GetProduct(new ProductIDRequest() { Id = id });

            if (data.Id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(data);
        }
    }
}