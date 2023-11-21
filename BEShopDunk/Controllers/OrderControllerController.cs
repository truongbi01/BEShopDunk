using BEShopDunk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEShopDunk.Controllers
{
    public class OrderControllerController : Controller
    {
        // GET: OrderController
        DBShopDunkEntities db= new DBShopDunkEntities();
        public ActionResult DisplayOrders()
        {
            // Lấy danh sách đơn hàng từ cơ sở dữ liệu
          var orderpro= db.OrderProes.ToList();
            return View(orderpro);
        }
        public ActionResult DisplayInfoCus()
        {
            var ab = db.Customers.ToList();
            return View(ab);
        }

    }
}