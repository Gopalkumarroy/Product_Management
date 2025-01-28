using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Product_management.Models;

namespace Product_management.Controllers
{
   
    public class ProductController : Controller
    {
        ProductMgmtDBEntities db = new ProductMgmtDBEntities();

        // GET: Product
        public ActionResult Index()
        {
            Product model = new Product();
            model.Categories = db.tblCategories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(model);
            
        }
        [HttpPost]
        public ActionResult Index(Product model)
        {
            if (ModelState.IsValid)
            {
                // Check if Product_Name already exists in the database
                if (db.tblProducts.Any(p => p.ProductName == model.ProductName))
                {
                    TempData["msg"] = "Product name already exists. Please choose a different name.";
                    model.Categories = db.tblCategories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
                    return View(model); // Return the view with the model to show the error
                }

                tblProduct tb = new tblProduct();
                tb.ProductId = model.ProductId;
                tb.CategoryId = model.CategoryId;
                tb.ProductName = model.ProductName;
                db.tblProducts.Add(tb);
                db.SaveChanges();
                TempData["MessageUPdate"] = "Item successfully Added!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["msg"] = "Not inserted Product";
            }
            return View();
        }

        //ViewPage of product
        public ActionResult ViewPage()
        {
            Product model = new Product();
            var query = db.tblProducts.ToList();//fetch product from database

            if (query != null)  
            {
                foreach (var item in query)
                {
                    Product cat = new Product();

                    cat.ProductName = item.ProductName;
                    cat.ProductId = item.ProductId;
                    cat.CategoryId = (int)item.CategoryId;
                    cat.categoryName = item.tblCategory.CategoryName.ToString();
                    
                    //Add the product to the list
                    model.Products.Add(cat);
                }
            }
            //Pass the list of product to the view
            return View(model);
        }

        //Edit the page 
        public ActionResult Edit(int id)
        {
            Product model = new Product();
            
            model.Categories = db.tblCategories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            var query = db.tblProducts.SingleOrDefault(x => x.ProductId == id);
            if (query != null)
            {
                model.ProductId = query.ProductId;
                model.ProductName = query.ProductName;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Product model)
        {
            // Find the existing product in the database
            var query = db.tblProducts.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (query != null)
            {
                // Update fields with values from the model
                query.ProductId = model.ProductId;
                query.ProductName = model.ProductName;
                query.CategoryId = model.CategoryId;
            
                // Save changes to the database
                db.SaveChanges();
                TempData["msg"] = "Product updated Successfuly!";
                return RedirectToAction("ViewPage");
            }

            TempData["msg"] = "Product could not be updated.";
            return View(model);
        }
        //Details the item from ViewPage
        public ActionResult Details(int id)
        {
            Product model = new Product();

            var detail = db.tblProducts.FirstOrDefault(x => x.ProductId == id);
            if (detail != null)
            {
                model.ProductId = detail.ProductId;
                model.categoryName = detail.tblCategory.CategoryName;
                model.CategoryId = (int)detail.CategoryId;
                model.ProductName = detail.ProductName;
            }
            return View(model);
        }
        //Delete the item
        public ActionResult Delete(int id)
        {
            var query = db.tblProducts.FirstOrDefault(x => x.ProductId == id);
            if (query != null)
            {
                db.tblProducts.Remove(query);
                db.SaveChanges();
            }
            return RedirectToAction("ViewPage");
        }


    }
}
