using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Product_management.Models
{
    public class Category
    {
        public Category()
        {
            Categories = new List<Category>();
        }
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<Category> Categories { get; set; }
    }
}