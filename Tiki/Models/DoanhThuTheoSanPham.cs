using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiki.Models
{
    public class DoanhThuTheoSanPham
    {
        public DateTime NgayBan { get; set; }
        public int IDDonHang { get; set; }
        public string TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public short SoLuong { get; set; }
        public decimal TongCong { get; set; }
    }
}