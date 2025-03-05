using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Product_management.Models;

namespace Product_management.Controllers
{
    [SessionAuthorize]
    public class ProductController : Controller
    {
        private readonly ProductMgmtDBEntities db = new ProductMgmtDBEntities(); // Define DB context once

        // GET: Product (With Pagination)
        [HttpGet]
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            // Total number of records
            int totalItems = db.tblProducts.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Fetch paginated products
            var products = db.tblProducts
                             .OrderBy(p => p.ProductId)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            // Map database products to the `Product` model
            List<Product> productList = products.Select(item => new Product
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                CategoryId = (int)item.CategoryId,
                categoryName = item.tblCategory != null ? item.tblCategory.CategoryName : "Unknown"
            }).ToList();

            // Pass pagination data to ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(productList); // ✅ Pass a List<Product>
        }


        // POST: Add Product
        [HttpPost]
        public ActionResult Index(Product model, int page = 1, int pageSize = 5)
        {
            if (ModelState.IsValid)
            {
                // Check if Product_Name already exists in the database
                if (db.tblProducts.Any(p => p.ProductName == model.ProductName))
                {
                    TempData["msg"] = "Product name already exists. Please choose a different name.";
                    return RedirectToAction("Index", new { page, pageSize });
                }

                // Save new product
                tblProduct tb = new tblProduct
                {
                    CategoryId = model.CategoryId,
                    ProductName = model.ProductName
                };
                db.tblProducts.Add(tb);
                db.SaveChanges();
                TempData["MessageUpdate"] = "Item successfully added!";
                return RedirectToAction("Index", new { page, pageSize });
            }
            TempData["msg"] = "Failed to insert product.";
            return RedirectToAction("Index", new { page, pageSize });
        }

        // View Products
        //public ActionResult ViewPage()
        //{
        //    var products = db.tblProducts.Select(item => new Product
        //    {
        //        ProductId = item.ProductId,
        //        ProductName = item.ProductName,
        //        CategoryId = (int)item.CategoryId,
        //        categoryName = item.tblCategory != null ? item.tblCategory.CategoryName : "Unknown"
        //    }).ToList();

        //    return View(products);
        //}
        //

        //// for paginatin
        //public ActionResult ViewPage(int page = 1, int pageSize = 5)
        //{
        //    // Total number of records
        //    int totalItems = db.tblProducts.Count();
        //    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    // Fetch paginated records
        //    var products = db.tblProducts
        //                     .OrderBy(p => p.ProductId)
        //                     .Skip((page - 1) * pageSize)
        //                     .Take(pageSize)
        //                     .Select(item => new Product
        //                     {
        //                         ProductId = item.ProductId,
        //                         ProductName = item.ProductName,
        //                         CategoryId = (int)item.CategoryId,
        //                         categoryName = item.tblCategory != null ? item.tblCategory.CategoryName : "Unknown"
        //                     })
        //                     .ToList();

        //    // Pass pagination data
        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.TotalItems = totalItems;

        //    return View(products);
        //}

        ////end pagination 


        // start Pagination with serach bar
        // for paginatin
        //public ActionResult ViewPage(string searchQuery = "", int page = 1, int pageSize = 5)
        //{
        //    var query = db.tblProducts.AsQueryable();

        //    // Apply search filter
        //    if (!string.IsNullOrEmpty(searchQuery))
        //    {
        //        query = query.Where(p => p.ProductName.Contains(searchQuery));
        //    }

        //    // Total records after filtering
        //    int totalItems = query.Count();
        //    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    // Fetch paginated & filtered records
        //    var products = query
        //                    .OrderBy(p => p.ProductId)
        //                    .Skip((page - 1) * pageSize)
        //                    .Take(pageSize)
        //                    .Select(item => new Product
        //                    {
        //                        ProductId = item.ProductId,
        //                        ProductName = item.ProductName,
        //                        CategoryId = (int)item.CategoryId,
        //                        categoryName = item.tblCategory != null ? item.tblCategory.CategoryName : "Unknown"
        //                    })
        //                    .ToList();

        //    // Pass data to view
        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.TotalItems = totalItems;
        //    ViewBag.SearchQuery = searchQuery;

        //    return View(products);
        //}

        //end pagination 
        //End Pagination with search bar

        //Starting of pagination with searching and sorting
        public ActionResult ViewPage(string searchQuery = "", int page = 1, int pageSize = 5)
        {
            var query = db.tblProducts.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.ProductName.Contains(searchQuery));
            }

            // Total records after filtering
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Fetch paginated records
            var products = query
                            .OrderBy(p => p.ProductId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(item => new Product
                            {
                                ProductId = item.ProductId,
                                ProductName = item.ProductName,
                                CategoryId = (int)item.CategoryId, 
                                categoryName = item.tblCategory != null ? item.tblCategory.CategoryName : "Unknown"
                            })
                            .ToList();

            // Pass data to view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchQuery = searchQuery;

            return View(products);
        }



        //Ending of pagination with searching and sorting

        // Edit Product (GET)
        public ActionResult Edit(int id)
        {
            var query = db.tblProducts.SingleOrDefault(x => x.ProductId == id);
            if (query == null) return HttpNotFound();

            Product model = new Product
            {
                ProductId = query.ProductId,
                ProductName = query.ProductName,
                Categories = db.tblCategories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };
            return View(model);
        }

        // Edit Product (POST)
        [HttpPost]
        public ActionResult Edit(Product model)
        {
            var query = db.tblProducts.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (query != null)
            {
                query.ProductName = model.ProductName;
                query.CategoryId = model.CategoryId;
                db.SaveChanges();
                TempData["msg"] = "Product updated successfully!";
                return RedirectToAction("ViewPage");
            }
            TempData["msg"] = "Product could not be updated.";
            return View(model);
        }

        // Product Details
        public ActionResult Details(int id)
        {
            var detail = db.tblProducts.FirstOrDefault(x => x.ProductId == id);
            if (detail == null) return HttpNotFound();

            Product model = new Product
            {
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                CategoryId = (int)detail.CategoryId,
                categoryName = detail.tblCategory != null ? detail.tblCategory.CategoryName : "Unknown"
            };
            return View(model);
        }

        // Delete Product
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
