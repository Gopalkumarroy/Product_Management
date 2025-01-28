using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Product_management.Models
{
    public class Product
    {
        public Product()
        {
            Categories = new List<SelectListItem>();
            Products = new List<Product>();
        }
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; }
        public string categoryName { get; set; }
        
        public List<SelectListItem> Categories { get; set; }

        public List<Product> Products { get; set; }

    }
}