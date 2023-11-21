using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEShopDunk.Models;
using System.Net;
using System.Data.Entity;

namespace BEShopDunk.Controllers
{
    public class CustomerProductsController : Controller
    {
        // GET: CustomerProducts
        private DBShopDunkEntities db= new DBShopDunkEntities();
        public ActionResult Index(string category)
        {
            if (category == null)
            {
                var productList = db.Products.OrderByDescending(p => p.NamePro);
                return View(productList);
            }
            else
            {
                var products = db.Products.OrderByDescending(p => p.NamePro).Where(p => p.Category == category);
                return View(products.ToList());
            }
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
       public ActionResult SearchOption(double min=double.MinValue,double max = double.MaxValue)
        {
            var list=db.Products.Where(p=>(double)p.Price>=min && (double)p.Price<=max).ToList();
            return View(list);
        }
        public ActionResult PartialCate()
        {
            return PartialView();
        }
        public ActionResult SearchNameOption(string TenSp)
        {
            var list = db.Products.Where(p => p.NamePro == TenSp).ToList();
            return View(list);
        }
    }
}