using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class CapNhatDHController : Controller
    {
        QL_PetShopEntities1 db = new QL_PetShopEntities1();

        //Hiển thị danh sách đơn hàng
        public ActionResult Index()
        {
            var list = db.tblHoaDon.OrderByDescending(h => h.NgayLap).ToList();
            return View(list);
        }
        //Load trang cập nhật
        public ActionResult CapNhat(int id)
        {
            var hd = db.tblHoaDon.Find(id);
            if (hd == null) return HttpNotFound();
            // Tạo dropdown tình trạng đơn hàng
            ViewBag.TinhTrang = new SelectList(db.tblTinhTrang.ToList(), "ID", "TinhTrangDonHang", hd.TinhTrang);
            return View(hd);
        }
        //Lưu cập nhật
        [HttpPost]
        public ActionResult CapNhat(tblHoaDon model)
        {
            var hd = db.tblHoaDon.Find(model.MaHD);
            if (hd == null) return HttpNotFound();
            hd.TinhTrang = model.TinhTrang;
            hd.DaThanhToan = model.DaThanhToan;
            hd.DiaChiGiaoHang = model.DiaChiGiaoHang;
            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật đơn hàng thành công!";
            return RedirectToAction("Index");
        }
    }
}