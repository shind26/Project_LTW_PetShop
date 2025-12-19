using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LapTrinhWeb.Controllers
{
    public class DangKyController : Controller
    {
        QL_PetShopEntities1 ql = new QL_PetShopEntities1();

        // GET: DangKy
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XuLyDangKy(tblKhachHang kh, string ReMatKhau)
        {
            var check = ql.tblKhachHang.FirstOrDefault(x => x.Email == kh.Email);
            if (check != null)
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View("Index", kh);
            }

            if (kh.MatKhau != ReMatKhau)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp!";
                return View("Index", kh);
            }

            ql.tblKhachHang.Add(kh);
            ql.SaveChanges();

            FormsAuthentication.SetAuthCookie(kh.Email, true);
            Session["MaKH"] = kh.MaKH;
            Session["TenKH"] = kh.TenKH;
            Session["Email"] = kh.Email;

            return Redirect("/");
        }

    }

}