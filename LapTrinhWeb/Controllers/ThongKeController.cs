using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class ThongKeController : Controller
    {
        // GET: ThongKe
        QL_PetShopEntities1 db = new QL_PetShopEntities1();
        // Trang thống kê mặc định (doanh thu hôm nay)
        public ActionResult Index(DateTime? from, DateTime? to)
        {
            // Nếu người dùng chưa chọn ngày thì lấy hôm nay
            if (!from.HasValue) from = DateTime.Today;
            if (!to.HasValue) to = DateTime.Today;
            // Lấy hóa đơn hoàn thành,4 = Hoàn thành

            var list = db.tblHoaDon.Where(h => h.NgayLap >= from && h.NgayLap <= to && h.TinhTrang == 4).ToList();
            // Tính tổng doanh thu
            ViewBag.TongDoanhThu = list.Sum(x => (decimal?)x.TongTien) ?? 0;
            // Gửi lại ngày cho View
            ViewBag.From = from.Value.ToString("yyyy-MM-dd");
            ViewBag.To = to.Value.ToString("yyyy-MM-dd");
            return View(list);
        }
    }
}