using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gRPC.Protos;
using Grpc.Core;
using gRPCService.Models;
using static gRPC.Protos.GrpcProduct;
using Microsoft.EntityFrameworkCore;

namespace gRPCService.Services
{
    public class ProductService : GrpcProductBase
    {
        public readonly gRPCContext _db;

        public ProductService(gRPCContext db)
        {
            _db = db;
        }

        public override Task<ProductList> GetAll(Empty requestData, ServerCallContext context)
        {
            ProductList response = new ProductList();

            var productList = from p in this._db.Products
                              join c in this._db.Categories on p.CategoryId equals c.Id
                              select new gRPC.Protos.ProductResponse()
                              {
                                  Id = p.Id,
                                  ProductName = p.ProductName,
                                  CategoryName = c.CategoryName,
                                  Price = (double)p.Price
                              };

            response.ProductResponses.AddRange(productList);
            return Task.FromResult(response);
        }

        public override Task<gRPC.Protos.ProductResponse> GetProduct(ProductIDRequest request, ServerCallContext context)
        {
            var objFromDb = from p in this._db.Products
                            join c in this._db.Categories on p.CategoryId equals c.Id
                            where p.Id == request.Id
                            select new gRPC.Protos.ProductResponse()
                            {
                                Id = p.Id,
                                ProductName = p.ProductName,
                                CategoryName = c.CategoryName,
                                Price = (double)p.Price                                
                            } as ProductResponse;

            if (objFromDb.FirstOrDefault() == null)
            {
                return Task.FromResult<gRPC.Protos.ProductResponse>(new ProductResponse());
            }            

            return Task.FromResult(objFromDb.FirstOrDefault());
        }

        public override Task<Result> CreateProduct(gRPC.Protos.Product request, ServerCallContext context)
        {
            try
            {
                Models.Product product = new()
                {
                    Id = request.Id,
                    ProductName = request.ProductName,
                    CategoryId = request.CategoryId,
                    Price = (decimal)request.Price
                };

                this._db.Products.Add(product);
                this._db.SaveChanges();

                gRPC.Protos.Result message = new()
                {
                    Message = "Product created successfully!!"
                };
                return Task.FromResult(message);
            }
            catch (Exception e)
            {
                gRPC.Protos.Result message = new()
                {
                    Message = e.Message
                };
                return Task.FromResult(message);
            }
        }

        public override Task<Result> UpdateProduct(gRPC.Protos.Product request, ServerCallContext context)
        {
            try
            {
                var prodFromDb = this._db.Products.AsNoTracking()
                    .SingleOrDefault(x => x.Id == request.Id);

                if (prodFromDb != null)
                {
                    Models.Product product = new()
                    {
                        Id = request.Id,
                        ProductName = request.ProductName,
                        CategoryId = request.CategoryId,
                        Price = (decimal)request.Price
                    };

                    this._db.Products.Update(product);
                    this._db.SaveChanges();

                    gRPC.Protos.Result success = new()
                    {
                        Message = "Product updated successfully!!"
                    };
                    return Task.FromResult(success);
                }

                gRPC.Protos.Result notExists = new()
                {
                    Message = "Product updated successfully!!"
                };
                return Task.FromResult(notExists);
            }
            catch (Exception e)
            {
                gRPC.Protos.Result message = new()
                {
                    Message = e.Message
                };
                return Task.FromResult(message);
            }
        }

        public override Task<Result> DeleteProduct(ProductIDRequest request, ServerCallContext context)
        {
            try
            {
                var prodFromDb = this._db.Products
                        .SingleOrDefault(x => x.Id == request.Id);

                this._db.Products.Remove(prodFromDb);
                this._db.SaveChanges();

                gRPC.Protos.Result message = new()
                {
                    Message = "Product removed successfully!!"
                };
                return Task.FromResult(message);
            }
            catch (Exception e)
            {
                gRPC.Protos.Result message = new()
                {
                    Message = e.Message
                };
                return Task.FromResult(message);
            }
        }
    }
}