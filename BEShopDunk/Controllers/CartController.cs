﻿using BEShopDunk.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace BEShopDunk.Controllers
{
    public class CartController : Controller
    {
        DBShopDunkEntities db= new DBShopDunkEntities();
        public List<CartItem> GetCart()
        {
            List<CartItem> myCart = Session["GioHang"] as
            List<CartItem>;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào Session
            if (myCart == null)
            {
                myCart = new List<CartItem>();
                Session["GioHang"] = myCart;
            }
            return myCart;
        }
        public ActionResult AddToCart(int id)
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p =>p.ProductID == id);
            if (currentProduct == null)
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }
            return RedirectToAction("Index", "CustomerProducts", new {  id = id});
        }
        private int GetTotalNumber()
        {
            int totalNumber = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalNumber = myCart.Sum(sp => sp.Number);
            return totalNumber;
        }
        private decimal GetTotalPrice()
        {
            decimal totalPrice = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalPrice = myCart.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }
        public ActionResult GetCartInfo()
        {
            List<CartItem> myCart = GetCart();
            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if (myCart == null || myCart.Count == 0)
            {
                return RedirectToAction("Index", "CustomerProducts");
            }
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
            return View(myCart); //Trả về View hiển thị thông tin giỏ hàng
        }
        public ActionResult CartPartial()
        {
            ViewBag.TotalNumber=GetTotalNumber();
            ViewBag.TotalPrice=GetTotalPrice();
            return PartialView();
        }
        public ActionResult DeleteCartItem(int id)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                myCart.RemoveAll(p => p.ProductID == id);
                return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
            }
            if (myCart.Count == 0) //Quay về trang chủ nếu giỏ hàng không có gì
                return RedirectToAction("Index", "CustomerProducts");
            return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
        }
        public ActionResult UpdateCartItem(int id, int Number)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                //Cập nhật lại số lượng tương ứng
                //Lưu ý số lượng phải lớn hơn hoặc bằng 1
                currentProduct.Number = Number;
            }
            return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
        }
        public ActionResult ConfirmCart()
        {
            if (Session["TaiKhoan"] == null) //Chưa đăng nhập
                return RedirectToAction("Login", "Users");
            List<CartItem> myCart = GetCart();
            if (myCart == null || myCart.Count == 0) //Chưa có giỏ hàng hoặcchưa có sp
            return RedirectToAction("Index", "CustomerProducts");
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
            return View(myCart); //Trả về View xác nhận đặt hàng
        }
        public ActionResult AgreeCart()
        {
            Customer khach = Session["TaiKhoan"] as Customer; //Khách
            List<CartItem> myCart = GetCart(); //Giỏ hàng
            OrderPro DonHang = new OrderPro(); //Tạo mới đơn đặt hàng
            
            DonHang.IDCus = khach.IDCus;
            DonHang.DateOrder = DateTime.Now;
            db.OrderProes.Add(DonHang);
            db.SaveChanges();
            //Lần lượt thêm từng chi tiết cho đơn hàng trên
            foreach (var product in myCart)
            {
                OrderDetail chitiet = new OrderDetail();
                chitiet.IDOrder = DonHang.ID;
                chitiet.IDProduct = product.ProductID;
                chitiet.Quantity = product.Number;
                chitiet.UnitPrice = (double)product.Price;
                db.OrderDetails.Add(chitiet);
            }
            db.SaveChanges();
            //Xóa giỏ hàng
            Session["GioHang"] = null;
            return RedirectToAction("Index", "CustomerProducts");
        }
        public ActionResult CheckOut(FormCollection form)
        {
            Customer khach = Session["TaiKhoan"] as Customer;
            OrderPro DonHang= new OrderPro();
            List<CartItem> myCart = GetCart();
            DonHang.AddressDeliverry = form["AddressDeliverry"];

            DonHang.IDCus = khach.IDCus;
            DonHang.DateOrder = DateTime.Now;
            db.OrderProes.Add(DonHang);
            db.SaveChanges();
            return RedirectToAction("GetCartInfo", "Cart");
            
        }
       
       

    }
}