using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Orders
    {
        [Key]
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}