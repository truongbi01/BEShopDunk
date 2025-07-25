﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BEShopDunk.Models;
using System.IO;

namespace BEShopDunk.Controllers
{
    public class ProductsController : Controller
    {
        private DBShopDunkEntities db = new DBShopDunkEntities();

        // GET: Products
        public ActionResult Index()
        {

            var products = db.Products.Include(p => p.Category1);
            return View(products.ToList());
        }

        // GET: Products/Details/5
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

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(db.Categories, "IDCate", "NameCate");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="ProductID,NamePro,DecriptionPro,Category,Price,ImagePro")] Product product,HttpPostedFileBase ImagePro)
        {
            if (ModelState.IsValid)
            {
                if (ImagePro != null)
                {
                    //Lấy tên file của hình được up lên

                    var fileName = Path.GetFileName(ImagePro.FileName);

                    //Tạo đường dẫn tới file

                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    //Lưu tên

                    product.ImagePro = fileName;
                    //Save vào Images Folder
                    ImagePro.SaveAs(path);

                }
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Categories, "IDCate", "NameCate",product.Category);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.Category = new SelectList(db.Categories, "IDCate", "NameCate", product.Category);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,NamePro,DecriptionPro,Category,Price,ImagePro")] Product product,
            HttpPostedFileBase ImagePro)
        {
            if (ModelState.IsValid)
            {
                var productDB=db.Products.FirstOrDefault(p=>p.ProductID==product.ProductID);
                if(productDB != null)
                {
                    productDB.NamePro = product.NamePro;
                    productDB.DecriptionPro=product.DecriptionPro;
                    productDB.Price=product.Price;
                    if (ImagePro != null)
                    {
                        //Lấy tên file của hình được up lên

                        var fileName = Path.GetFileName(ImagePro.FileName);

                        //Tạo đường dẫn tới file

                        var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                        //Lưu tên

                        productDB.ImagePro = fileName;
                        //Save vào Images Folder
                        ImagePro.SaveAs(path);
                    }
                    productDB.Category = product.Category;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Categories, "IDCate", "NameCate", product.Category);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
