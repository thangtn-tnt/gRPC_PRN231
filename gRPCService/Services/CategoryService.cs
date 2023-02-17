using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gRPC.Protos;
using Grpc.Core;
using gRPCService.Models;
using static gRPC.Protos.GrpcCategory;

namespace gRPCService.Services
{
    public class CategoryService : GrpcCategoryBase
    {
        public readonly gRPCContext _db;

        public CategoryService(gRPCContext db)
        {
            _db = db;
        }

        public override Task<CategoryList> GetAll(EmptyCate requestData, ServerCallContext context)
        {
            CategoryList response = new CategoryList();

            var cusList = from o in this._db.Categories
                          select new gRPC.Protos.Category()
                          {
                              Id = o.Id,
                              CategoryName = o.CategoryName,
                          };

            response.Categories.AddRange(cusList);
            return Task.FromResult(response);
        }

        public override Task<gRPC.Protos.Category> GetCategory(CateIDRequest request, ServerCallContext context)
        {
            var objFromDb = this._db.Categories.Find(request.Id);

            gRPC.Protos.Category cus = new()
            {
                Id = objFromDb.Id,
                CategoryName = objFromDb.CategoryName
            };

            return Task.FromResult(cus);
        }
    }
}