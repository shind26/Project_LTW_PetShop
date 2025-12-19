using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
      QL_PetShopEntities1 pet = new QL_PetShopEntities1();
        // Hàm bỏ dấu tiếng Việt
        public static string RemoveVietnamese(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.ToLower();

            string[] signs = new string[]
            {
                "aáàảạãăắằẳẵặâấầẩẫậ",
                "eéèẻẽẹêếềểễệ",
                "iíìỉĩị",
                "oóòỏõọôốồổỗộơớờởỡợ",
                "uúùủũụưứừửữự",
                "yýỳỷỹỵ",
                "dđ"
            };

            string[] replaces = new string[]
            {
                "a", "e", "i", "o", "u", "y", "d"
            };

            for (int i = 0; i < signs.Length; i++)
            {
                foreach (var c in signs[i])
                {
                    str = str.Replace(c, replaces[i][0]);
                }
            }

            return str;
        }

        public ActionResult Index(string loai = "", string searchString = "")
        {
            List<tblSanPham> sp = pet.tblSanPham.ToList();


            // Tìm kiếm nâng cao: không dấu + nhiều từ khóa + tên + mô tả + loại
            if (!string.IsNullOrEmpty(searchString))
            {
                string keyword = RemoveVietnamese(searchString);
                var words = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                sp = sp.Where(s =>
                {
                    string name = RemoveVietnamese(s.TenSP);
                    string mota = RemoveVietnamese(s.MoTa ?? "");
                    string loai1 = RemoveVietnamese(s.LoaiSanPham);
                    return words.All(w =>
                        name.Contains(w) ||
                        mota.Contains(w) ||
                        loai1.Contains(w)
                    );
                }).ToList();
            }



            // Filter theo loại nếu có
            if (!string.IsNullOrEmpty(loai))
            {
                switch (loai.ToLower())
                {
                    case "cho":
                        sp = sp.Where(s => s.LoaiSanPham == "Pet" && s.TenSP.ToLower().Contains("chó")).ToList();
                        break;
                    case "meo":
                        sp = sp.Where(s => s.LoaiSanPham == "Pet" && s.TenSP.ToLower().Contains("mèo")).ToList();
                        break;
                    case "thucancho":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" && s.TenSP.ToLower().Contains("thức ăn") && s.TenSP.ToLower().Contains("chó")).ToList();
                        break;
                    case "thucanmeo":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" && s.TenSP.ToLower().Contains("thức ăn") && s.TenSP.ToLower().Contains("mèo")).ToList();
                        break;
                    case "pate":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" && s.TenSP.ToLower().Contains("pate")).ToList();
                        break;
                    case "docho":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" &&
                                          (s.TenSP.ToLower().Contains("đồ chơi") ||
                                           s.TenSP.ToLower().Contains("bóng") ||
                                           s.TenSP.ToLower().Contains("gặm"))).ToList();
                        break;
                    case "daydat":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" &&
                                          (s.TenSP.ToLower().Contains("dây") ||
                                           s.TenSP.ToLower().Contains("dắt") ||
                                           s.TenSP.ToLower().Contains("vòng"))).ToList();
                        break;
                    case "quanao":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" &&
                                          (s.TenSP.ToLower().Contains("áo") ||
                                           s.TenSP.ToLower().Contains("quần"))).ToList();
                        break;
                    case "chuongngu":
                        sp = sp.Where(s => s.LoaiSanPham == "Accessory" &&
                                          (s.TenSP.ToLower().Contains("chuồng") ||
                                           s.TenSP.ToLower().Contains("nệm") ||
                                           s.TenSP.ToLower().Contains("ngủ"))).ToList();
                        break;
                    case "dichvu":
                        sp = sp.Where(s => s.LoaiSanPham == "Service").ToList();
                        break;
                }
            }

            // Tách danh sách theo loại sản phẩm
            var pets = sp.Where(s => s.LoaiSanPham == "Pet").ToList();
            var accessories = sp.Where(s => s.LoaiSanPham == "Accessory").ToList();
            var services = sp.Where(s => s.LoaiSanPham == "Service").ToList();

            ViewBag.Pets = pets;
            ViewBag.Accessories = accessories;
            ViewBag.Services = services;
            ViewBag.CurrentCategory = loai;
            ViewBag.SearchString = searchString;

            return View(sp);
        }
        public ActionResult DichVu()
        {
            List<tblDichVu> dvu = pet.tblDichVu.ToList();
            return View(dvu);
        }
    }
}