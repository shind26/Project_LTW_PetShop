using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LapTrinhWeb.Controllers
{
    public class DangNhapController : Controller
    {
        QL_PetShopEntities1 ql = new QL_PetShopEntities1();

        // GET: DangNhap
        public ActionResult Index(string returnUrl)
        {
            // nếu returnUrl null → gán mặc định "/"
            ViewBag.Url = string.IsNullOrEmpty(returnUrl)
                          ? "/"
                          : returnUrl;

            return View();
        }

        [HttpPost]
        public ActionResult XuLyDN(string email, string password, string Loai, string url)
        {
            var kq = ql.tblKhachHang.FirstOrDefault(kh => kh.Email == email && kh.MatKhau == password);

            if (kq == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
                ViewBag.Url = url;   // GIỮ LẠI URL
                return View("Index");
            }

            // Lưu cookie + session
            FormsAuthentication.SetAuthCookie(kq.Email, true);

            Session["MaKH"] = kq.MaKH;
            Session["TenKH"] = kq.TenKH;
            Session["Email"] = kq.Email;

            if (string.IsNullOrEmpty(url))
                url = "/";

            return Redirect(url);
        }
        
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}
