using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapTrinhWeb.Models
{
	public class CartItem
	{
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string AnhDaiDien { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public string LoaiSanPham { get; set; }

        public decimal ThanhTien
        {
            get { return GiaBan * SoLuong; }
        }
    }
}