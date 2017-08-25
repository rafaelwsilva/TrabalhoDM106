using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DM106RafaelSilva.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime DateOrder { get; set; }

        public DateTime? DateDelivery { get; set; }

        public string Status { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal TotalWeight { get; set; }

        public decimal Shipping { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}