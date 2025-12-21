using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class QLKhachHangController : Controller
    {
        // GET: QLKhachHang
        private QL_PetShopEntities1 db = new QL_PetShopEntities1();

        // 1. HIỂN THỊ DANH SÁCH KHÁCH HÀNG
        public ActionResult Index()
        {
            var dsKhachHang = db.tblKhachHang.ToList();
            return View(dsKhachHang);
        }

        // 2. XEM ĐƠN HÀNG CỦA KHÁCH HÀNG
        public ActionResult XemDonHang(int id)
        {
            var khachHang = db.tblKhachHang.Find(id);
            if (khachHang == null) return HttpNotFound();

            // Lấy danh sách hóa đơn của khách hàng này
            var dsHoaDon = db.tblHoaDon.Where(h => h.MaKH == id).OrderByDescending(h => h.NgayLap).ToList();

            ViewBag.TenKhachHang = khachHang.TenKH;
            return View(dsHoaDon);
        }
    }
}