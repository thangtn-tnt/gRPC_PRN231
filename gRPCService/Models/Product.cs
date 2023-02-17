using System;
using System.Collections.Generic;

#nullable disable

namespace gRPCService.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }

        public virtual Category Category { get; set; }
    }
}
