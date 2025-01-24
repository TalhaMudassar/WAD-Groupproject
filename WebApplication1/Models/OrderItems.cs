using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class OrderItems
    {
        [Key]
        public int Id { get; set; } // primary Key
        public int OrderId { get; set; } // foreign Key 
        public int ProductId { get; set; } // foreign Key 
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation Properties
        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}