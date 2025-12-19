using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb.Models;

namespace LapTrinhWeb.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        private QL_PetShopEntities1 db = new QL_PetShopEntities1();

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            decimal tongTien = cart.Sum(x => x.ThanhTien);
            ViewBag.TongTien = tongTien;
            ViewBag.SoLuongSP = cart.Sum(x => x.SoLuong);
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult ThemVaoGio(int maSP, int soLuong = 1)
        {
            var sanPham = db.tblSanPham.Find(maSP);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra tồn kho
            var tonKho = db.tblTonKho.Find(maSP);
            if (tonKho == null)
            {
                TempData["Loi"] = "Sản phẩm không có sẵn trong kho!";
                return RedirectToAction("Index");
            }

            // Kiểm tra theo loại sản phẩm
            if (sanPham.LoaiSanPham == "Pet")
            {
                if (tonKho.TrangThai != "Sẵn sàng bán")
                {
                    TempData["Loi"] = "Thú cưng này không còn sẵn để bán!";
                    return RedirectToAction("Index");
                }
                soLuong = 1; // Pet chỉ được mua 1
            }
            else if (sanPham.LoaiSanPham == "Accessory")
            {
                if (tonKho.SoLuongTon < soLuong)
                {
                    TempData["Loi"] = $"Chỉ còn {tonKho.SoLuongTon} sản phẩm trong kho!";
                    return RedirectToAction("Index");
                }
            }

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == maSP);

            if (item != null)
            {
                // Kiểm tra lại số lượng khi tăng
                if (sanPham.LoaiSanPham == "Accessory")
                {
                    if (item.SoLuong + soLuong > tonKho.SoLuongTon)
                    {
                        TempData["Loi"] = $"Chỉ còn {tonKho.SoLuongTon} sản phẩm trong kho!";
                        return RedirectToAction("Index");
                    }
                    item.SoLuong += soLuong;
                }
                else
                {
                    TempData["Loi"] = "Thú cưng đã có trong giỏ hàng!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaSP = sanPham.MaSP,
                    TenSP = sanPham.TenSP,
                    AnhDaiDien = sanPham.AnhDaiDien,
                    GiaBan = sanPham.GiaBan ?? 0,
                    SoLuong = soLuong,
                    LoaiSanPham = sanPham.LoaiSanPham
                });
            }

            Session["Cart"] = cart;
            TempData["ThongBao"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm
        [HttpPost]
        public ActionResult CapNhatSoLuong(int maSP, int soLuong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == maSP);

            if (item != null)
            {
                if (soLuong > 0)
                {
                    // Kiểm tra tồn kho nếu là phụ kiện
                    if (item.LoaiSanPham == "Accessory")
                    {
                        var tonKho = db.tblTonKho.Find(maSP);
                        if (tonKho != null && soLuong > tonKho.SoLuongTon)
                        {
                            TempData["Loi"] = $"Chỉ còn {tonKho.SoLuongTon} sản phẩm trong kho!";
                            return RedirectToAction("Index");
                        }
                    }
                    item.SoLuong = soLuong;
                }
                else
                {
                    cart.Remove(item);
                }
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaSanPham(int maSP)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == maSP);

            if (item != null)
            {
                cart.Remove(item);
            }

            Session["Cart"] = cart;
            TempData["ThongBao"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public ActionResult XoaGioHang()
        {
            Session["Cart"] = null;
            TempData["ThongBao"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index");
        }

        // Hiển thị form đặt hàng
        public ActionResult DatHang()
        {
            var cart = GetCart();
            if (cart == null || cart.Count == 0)
            {
                TempData["LoiDatHang"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Kiểm tra đăng nhập
            if (Session["MaKH"] == null)
            {
                TempData["LoiDatHang"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("DangNhap", "KhachHang", new { returnUrl = Url.Action("DatHang", "Cart") });
            }

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var khachHang = db.tblKhachHang.Find(maKH);

            ViewBag.KhachHang = khachHang;
            ViewBag.Cart = cart;
            ViewBag.TongTien = cart.Sum(x => x.ThanhTien);

            return View();
        }

        // Xử lý đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanDatHang(string diaChiGiaoHang, string maGiamGia, string ghiChu)
        {
            var cart = GetCart();
            if (cart == null || cart.Count == 0)
            {
                TempData["LoiDatHang"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            if (Session["MaKH"] == null)
            {
                TempData["LoiDatHang"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // Kiểm tra tồn kho trước khi đặt hàng
            foreach (var item in cart)
            {
                var tonKho = db.tblTonKho.Find(item.MaSP);
                if (tonKho == null)
                {
                    TempData["LoiDatHang"] = $"Sản phẩm {item.TenSP} không có trong kho!";
                    return RedirectToAction("Index");
                }

                if (item.LoaiSanPham == "Accessory" && tonKho.SoLuongTon < item.SoLuong)
                {
                    TempData["LoiDatHang"] = $"Sản phẩm {item.TenSP} không đủ số lượng! Chỉ còn {tonKho.SoLuongTon}.";
                    return RedirectToAction("Index");
                }

                if (item.LoaiSanPham == "Pet" && tonKho.TrangThai != "Sẵn sàng bán")
                {
                    TempData["LoiDatHang"] = $"Thú cưng {item.TenSP} không còn sẵn để bán!";
                    return RedirectToAction("Index");
                }
            }

            int maKH = Convert.ToInt32(Session["MaKH"]);
            decimal tongTien = cart.Sum(x => x.ThanhTien);
            decimal tienGiam = 0;

            // Xử lý mã giảm giá
            if (!string.IsNullOrEmpty(maGiamGia))
            {
                var maGG = db.tblMaGiamGia.FirstOrDefault(x => x.MaGiamGia == maGiamGia
                    && x.NgayBatDau <= DateTime.Now
                    && x.NgayKetThuc >= DateTime.Now
                    && (x.SoLuong == null || x.SoLuong > 0));

                if (maGG != null)
                {
                    tienGiam = tongTien * (maGG.PhanTramGiam ?? 0) / 100;
                    if (maGG.SoTienGiamToiDa != null && tienGiam > maGG.SoTienGiamToiDa)
                    {
                        tienGiam = maGG.SoTienGiamToiDa.Value;
                    }

                    // Giảm số lượng mã giảm giá
                    if (maGG.SoLuong != null)
                    {
                        maGG.SoLuong--;
                    }
                }
                else
                {
                    TempData["ThongBao"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn!";
                }
            }
            // Mở kết nối trước khi bắt đầu transaction
            if (db.Database.Connection.State != System.Data.ConnectionState.Open)
            {
                db.Database.Connection.Open();
            }
            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
            using (var transaction = db.Database.Connection.BeginTransaction())
            {
                try
                {
                    // Tạo hóa đơn
                    var hoaDon = new tblHoaDon
                    {
                        MaKH = maKH,
                        NgayLap = DateTime.Now,
                        TongTien = tongTien - tienGiam,
                        TinhTrang = 10, // Chờ xác nhận
                        DiaChiGiaoHang = diaChiGiaoHang,
                        DaThanhToan = false,
                        MaGiamGia = string.IsNullOrEmpty(maGiamGia) ? null : maGiamGia,
                        TienGiam = tienGiam > 0 ? tienGiam : (decimal?)null
                    };

                    db.tblHoaDon.Add(hoaDon);
                    db.SaveChanges();

                    // Thêm chi tiết hóa đơn và cập nhật tồn kho
                    foreach (var item in cart)
                    {
                        var chiTiet = new tblChiTietHoaDon
                        {
                            MaHD = hoaDon.MaHD,
                            MaSP = item.MaSP,
                            SoLuong = item.SoLuong,
                            GiaBan = item.GiaBan
                        };
                        db.tblChiTietHoaDon.Add(chiTiet);

                        // Cập nhật tồn kho
                        var tonKho = db.tblTonKho.Find(item.MaSP);
                        if (tonKho != null)
                        {
                            if (item.LoaiSanPham == "Accessory")
                            {
                                tonKho.SoLuongTon -= item.SoLuong;
                            }
                            else if (item.LoaiSanPham == "Pet")
                            {
                                tonKho.TrangThai = "Đã bán";
                            }
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // Xóa giỏ hàng
                    Session["Cart"] = null;

                    TempData["ThanhCong"] = "Đặt hàng thành công! Mã đơn hàng: " + hoaDon.MaHD;
                    return RedirectToAction("ChiTietDonHang", new { id = hoaDon.MaHD });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["LoiDatHang"] = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại!";
                    return RedirectToAction("Index");
                }
            }
        }

        // Xem chi tiết đơn hàng
        public ActionResult ChiTietDonHang(int id)
        {
            var hoaDon = db.tblHoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra quyền xem
            if (Session["MaKH"] == null || Convert.ToInt32(Session["MaKH"]) != hoaDon.MaKH)
            {
                TempData["Loi"] = "Bạn không có quyền xem đơn hàng này!";
                return RedirectToAction("Index", "Home");
            }

            return View(hoaDon);
        }

        // Hủy đơn hàng
        public ActionResult HuyDonHang(int id)
        {
            var hoaDon = db.tblHoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra quyền hủy
            if (Session["MaKH"] == null || Convert.ToInt32(Session["MaKH"]) != hoaDon.MaKH)
            {
                TempData["Loi"] = "Bạn không có quyền hủy đơn hàng này!";
                return RedirectToAction("Index", "Home");
            }

            // Chỉ cho phép hủy nếu đơn hàng đang xử lý (1) hoặc chờ xác nhận (10)
            if (hoaDon.TinhTrang == 1 || hoaDon.TinhTrang == 10)
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    db.Database.Connection.Open();
                }
                // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
                using (var transaction = db.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        hoaDon.TinhTrang = 5; // Đã hủy

                        // Hoàn trả tồn kho
                        var chiTietList = db.tblChiTietHoaDon.Where(x => x.MaHD == id).ToList();
                        foreach (var ct in chiTietList)
                        {
                            var tonKho = db.tblTonKho.Find(ct.MaSP);
                            var sanPham = db.tblSanPham.Find(ct.MaSP);

                            if (tonKho != null && sanPham != null)
                            {
                                if (sanPham.LoaiSanPham == "Accessory")
                                {
                                    tonKho.SoLuongTon += ct.SoLuong;
                                }
                                else if (sanPham.LoaiSanPham == "Pet")
                                {
                                    tonKho.TrangThai = "Sẵn sàng bán";
                                }
                            }
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        TempData["ThanhCong"] = "Đã hủy đơn hàng thành công!";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["Loi"] = "Có lỗi xảy ra khi hủy đơn hàng!";
                    }
                }
            }
            else
            {
                TempData["Loi"] = "Không thể hủy đơn hàng này! Đơn hàng đang được xử lý hoặc đã hoàn thành.";
            }

            return RedirectToAction("ChiTietDonHang", new { id = id });
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