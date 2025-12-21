using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb;

namespace LapTrinhWeb.Controllers
{
    public class QLNhanVienController : Controller
    {
        // GET: QLNhanVien
        private QL_PetShopEntities1 db = new QL_PetShopEntities1();

        // GET: NhanVien
        public ActionResult Index()
        {
            var tblNhanVien = db.tblNhanVien.Include(t => t.tblVaiTro);
            return View(tblNhanVien.ToList());
        }

        // GET: NhanVien/Create
        public ActionResult Create()
        {
            ViewBag.VaiTro = new SelectList(db.tblVaiTro, "IDVaiTro", "TenVaiTro");
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNV,MatKhau,TenNV,GioiTinh,NamSinh,VaiTro")] tblNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                db.tblNhanVien.Add(nhanVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VaiTro = new SelectList(db.tblVaiTro, "IDVaiTro", "TenVaiTro", nhanVien.VaiTro);
            return View(nhanVien);
        }

        // GET: NhanVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            tblNhanVien nhanVien = db.tblNhanVien.Find(id);
            if (nhanVien == null) return HttpNotFound();

            ViewBag.VaiTro = new SelectList(db.tblVaiTro, "IDVaiTro", "TenVaiTro", nhanVien.VaiTro);
            return View(nhanVien);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int MaNV, int VaiTro)
        {
            // Tìm nhân viên trong DB
            var nvInDb = db.tblNhanVien.Find(MaNV);
            if (nvInDb == null) return HttpNotFound();

            // CHỈ CẬP NHẬT VAI TRÒ
            nvInDb.VaiTro = VaiTro;

            // Đánh dấu thực thể đã thay đổi
            db.Entry(nvInDb).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: NhanVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblNhanVien nhanVien = db.tblNhanVien.Find(id);
            if (nhanVien == null) return HttpNotFound();
            return View(nhanVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblNhanVien nhanVien = db.tblNhanVien.Find(id);
            db.tblNhanVien.Remove(nhanVien);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}