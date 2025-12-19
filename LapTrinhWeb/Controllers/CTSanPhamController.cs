using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class CTSanPhamController : Controller
    {
        // GET: CTSanPham
        QL_PetShopEntities1 pet = new QL_PetShopEntities1();
        public ActionResult Index()
        {
            List<tblSanPham> lstPet = pet.tblSanPham.OrderByDescending(d => d.GiaBan).ToList();
            return View(lstPet);
        }

        public ActionResult ChiTiet(int masp)
        {
            var pet_detail = pet.tblSanPham.FirstOrDefault(d => d.MaSP == masp);
            //thumbnail
            var hinhAnhList = pet.tblHinhAnh.Where(h => h.MaSP == masp).ToList();
            ViewBag.HinhAnhList = hinhAnhList;
            //Cmts
            var binhluanList = pet.tblBinhLuan.Where(b => b.MaSP == masp).OrderByDescending(b => b.Ngay).ToList();

            ViewBag.BinhLuanList = binhluanList;
            //SPLQ
            List<tblSanPham> petlq = pet.tblSanPham.Where(d => d.MaDanhMuc == pet_detail.MaDanhMuc && d.MaSP != masp).ToList();
            ViewBag.PetLQ = petlq;
            //SPLQ
            List<tblSanPham> sachnxb = pet.tblSanPham.Where(d => d.MaNCC == pet_detail.MaNCC && d.MaSP != masp).ToList();
            ViewBag.PetNCC = sachnxb;
            return View(pet_detail);
        }

        [HttpPost]
        public ActionResult GuiBinhLuan(tblBinhLuan binhluan)
        {
            binhluan.Ngay = DateTime.Now;
            pet.tblBinhLuan.Add(binhluan);
            pet.SaveChanges();

            return RedirectToAction("ChiTiet", new { masp = binhluan.MaSP });
        }
    }
}