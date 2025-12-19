using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private QL_PetShopEntities1 db = new QL_PetShopEntities1();


        public ActionResult Index()
        {
            try
            {
                // Thống kê nhanh cho dashboard
                ViewBag.TongSanPham = db.tblSanPham.Count();
                ViewBag.TongThuCung = db.tblSanPham.Count(p => p.LoaiSanPham == "Pet");
                ViewBag.TongPhuKien = db.tblSanPham.Count(p => p.LoaiSanPham == "Accessory");
                ViewBag.TongKhachHang = db.tblKhachHang.Count();
                ViewBag.TongDonHang = db.tblHoaDon.Count();
                ViewBag.TongNhaCungCap = db.tblNhaCungCap.Count();

                // Lấy 5 sản phẩm mới nhất
                ViewBag.SanPhamMoiNhat = db.tblSanPham
                    .OrderByDescending(p => p.MaSP)
                    .Take(5)
                    .ToList();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View();
            }
        }


        public ActionResult SanPham(string loaiFilter = "", string search = "")
        {
            try
            {
                var query = db.tblSanPham.AsQueryable();

                // Lọc theo loại sản phẩm
                if (!string.IsNullOrEmpty(loaiFilter))
                {
                    query = query.Where(p => p.LoaiSanPham == loaiFilter);
                }

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.TenSP.Contains(search) ||
                                             p.MoTa.Contains(search));
                }

                // Load thông tin liên quan
                var sanPhamList = query.Include("tblDanhMuc")
                                       .Include("tblNhaCungCap")
                                       .Include("tblTonKho")
                                       .OrderByDescending(p => p.MaSP)
                                       .ToList();

                ViewBag.LoaiFilter = loaiFilter;
                ViewBag.Search = search;
                ViewBag.LoaiSanPhamList = new SelectList(new[]
                {
                    new { Value = "", Text = "Tất cả" },
                    new { Value = "Pet", Text = "Thú cưng" },
                    new { Value = "Accessory", Text = "Phụ kiện" }
                }, "Value", "Text", loaiFilter);

                return View(sanPhamList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View();
            }
        }

        public ActionResult SanPhamCreate()
        {
            try
            {
                ViewBag.MaDanhMuc = new SelectList(db.tblDanhMuc, "MaDanhMuc", "TenDanhMuc");
                ViewBag.MaNCC = new SelectList(db.tblNhaCungCap, "MaNCC", "TenNCC");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("SanPham");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SanPhamCreate(tblSanPham sanPham)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.tblSanPham.Add(sanPham);
                    db.SaveChanges();

                    // Tạo bản ghi tồn kho
                    var tonKho = new tblTonKho
                    {
                        MaSP = sanPham.MaSP,
                        SoLuongTon = sanPham.LoaiSanPham == "Accessory" ? 0 : (int?)null,
                        TrangThai = sanPham.LoaiSanPham == "Pet" ? "Sẵn sàng bán" : null
                    };
                    db.tblTonKho.Add(tonKho);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("SanPham");
                }

                ViewBag.MaDanhMuc = new SelectList(db.tblDanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNCC = new SelectList(db.tblNhaCungCap, "MaNCC", "TenNCC", sanPham.MaNCC);
                return View(sanPham);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                ViewBag.MaDanhMuc = new SelectList(db.tblDanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNCC = new SelectList(db.tblNhaCungCap, "MaNCC", "TenNCC", sanPham.MaNCC);
                return View(sanPham);
            }
        }


        public ActionResult SanPhamEdit(int id)
        {
            try
            {
                tblSanPham sanPham = db.tblSanPham.Find(id);
                if (sanPham == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("SanPham");
                }

                ViewBag.MaDanhMuc = new SelectList(db.tblDanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNCC = new SelectList(db.tblNhaCungCap, "MaNCC", "TenNCC", sanPham.MaNCC);
                return View(sanPham);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("SanPham");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SanPhamEdit(tblSanPham sanPham)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm sản phẩm cũ và cập nhật
                    var existing = db.tblSanPham.Find(sanPham.MaSP);
                    if (existing != null)
                    {
                        existing.TenSP = sanPham.TenSP;
                        existing.LoaiSanPham = sanPham.LoaiSanPham;
                        existing.GioiTinh = sanPham.GioiTinh;
                        existing.NgaySinh = sanPham.NgaySinh;
                        existing.MauSac = sanPham.MauSac;
                        existing.TinhTrangSucKhoe = sanPham.TinhTrangSucKhoe;
                        existing.TienSuBenh = sanPham.TienSuBenh;
                        existing.MoTa = sanPham.MoTa;
                        existing.GiaBan = sanPham.GiaBan;
                        existing.AnhDaiDien = sanPham.AnhDaiDien;
                        existing.MaDanhMuc = sanPham.MaDanhMuc;
                        existing.MaNCC = sanPham.MaNCC;

                        db.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("SanPham");
                }

                ViewBag.MaDanhMuc = new SelectList(db.tblDanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNCC = new SelectList(db.tblNhaCungCap, "MaNCC", "TenNCC", sanPham.MaNCC);
                return View(sanPham);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                ViewBag.MaDanhMuc = new SelectList(db.tblDanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                ViewBag.MaNCC = new SelectList(db.tblNhaCungCap, "MaNCC", "TenNCC", sanPham.MaNCC);
                return View(sanPham);
            }
        }
        public ActionResult SanPhamDetails(int id)
        {
            try
            {
                var sanPham = db.tblSanPham
                    .Include("tblDanhMuc")
                    .Include("tblNhaCungCap")
                    .Include("tblTonKho")
                    .FirstOrDefault(p => p.MaSP == id);

                if (sanPham == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("SanPham");
                }

                return View(sanPham);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult SanPhamDelete(int id)
        {
            try
            {
                var sanPham = db.tblSanPham
                    .Include("tblDanhMuc")
                    .Include("tblNhaCungCap")
                    .FirstOrDefault(p => p.MaSP == id);

                if (sanPham == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("SanPham");
                }

                return View(sanPham);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("SanPham");
            }
        }

        // POST: /Admin/SanPhamDelete/5
        [HttpPost, ActionName("SanPhamDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SanPhamDeleteConfirmed(int id)
        {
            try
            {
                // Kiểm tra xem sản phẩm có trong hóa đơn không
                var coTrongHoaDon = db.tblChiTietHoaDon.Any(x => x.MaSP == id);
                if (coTrongHoaDon)
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm đã có trong hóa đơn!";
                    return RedirectToAction("SanPham");
                }

                // Xóa tồn kho trước
                var tonKho = db.tblTonKho.Find(id);
                if (tonKho != null)
                {
                    db.tblTonKho.Remove(tonKho);
                }

                // Xóa sản phẩm
                tblSanPham sanPham = db.tblSanPham.Find(id);
                db.tblSanPham.Remove(sanPham);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
            }

            return RedirectToAction("SanPham");
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