using Product_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Product_management.Controllers
{
    public class CategoryController : Controller
    {
        ProductMgmtDBEntities db = new ProductMgmtDBEntities();
        // GET: Category
        public ActionResult Index()
        {
            return View(); 
        }

       [HttpPost]
        public ActionResult Index(Category model)
        {
            if (ModelState.IsValid)
            {
                tblCategory tb = new tblCategory();
                tb.CategoryName = model.CategoryName;
                db.tblCategories.Add(tb);
                db.SaveChanges();
                TempData["MessageUPdate"] = "Item successfully Added!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["msg"] = "Not inserted Category";
            }
            return View();
        }
        //ViewPage for Category
        public ActionResult ViewPage()
        {
            Category model = new Category();
            var query = db.tblCategories.ToList();
            if (query != null)
            {
                foreach (var item in query)
                {
                    Category mod = new Category();
                    mod.CategoryId = item.CategoryId;
                    mod.CategoryName = item.CategoryName;
                    model.Categories.Add(mod);
                }
            }
            return View(model);
        }
        //Edit the page
        public ActionResult Edit(int id)
        {
            Category model = new Category();
            var query = db.tblCategories.SingleOrDefault(x => x.CategoryId == id);
            if (query != null)
            {
                model.CategoryId = query.CategoryId;
                model.CategoryName = query.CategoryName;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Category model)
        {
            var query = db.tblCategories.FirstOrDefault(c => c.CategoryId == model.CategoryId);
            query.CategoryName = model.CategoryName;
            db.SaveChanges();
            TempData["MessageUPdate"] = "Item successfully updated!";
            return RedirectToAction("ViewPage", new { categoryId = model.CategoryId });
        }
        //For DetalPage 
        public ActionResult DetailPage(int id)
        {
            Category model = new Category();
            var detail = db.tblCategories.FirstOrDefault(x => x.CategoryId == id);
            if (detail != null)
            {
                //model.CatId = detail.CatId;
                model.CategoryName = detail.CategoryName;
            }
            return View(model);
        }
        //Delete the page

        public ActionResult Delete(int id)
        {
            var query = db.tblCategories.FirstOrDefault(x => x.CategoryId == id);
            if (query != null)
            {
                db.tblCategories.Remove(query);
                db.SaveChanges();
                TempData["Message"] = "Record deleted successfully.";
            }
            return RedirectToAction("ViewPage");
        }
    }
} 