//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tiki.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SanPham
    {
        public SanPham()
        {
            this.BinhLuans = new HashSet<BinhLuan>();
            this.GioHangs = new HashSet<GioHang>();
            this.HoaDons = new HashSet<HoaDon>();
        }
    
        public int MaSP { get; set; }

        [Required]
        public string TenSanPham { get; set; }
        [Required]
        public string MoTa { get; set; }
        [Required]
        public string HinhAnh1 { get; set; }
        [Required]
        public string HinhAnh2 { get; set; }
        [Required]
        public string HinhAnh3 { get; set; }
        [Required]
        public Nullable<decimal> Gia { get; set; }
  
        public string ThuongHieu { get; set; }

        public string XuatXu { get; set; }
        public string Camera { get; set; }
        public string CPU { get; set; }
        public string ChipDoHoa { get; set; }
        public Nullable<short> RAM { get; set; }
        public Nullable<short> ROM { get; set; }
        public Nullable<decimal> KichThuocMH { get; set; }
        public Nullable<decimal> KhoiLuong { get; set; }
        public string CongSuat { get; set; }
        public Nullable<short> BaoHanh { get; set; }
        public string ChucNangKhac { get; set; }
        public Nullable<int> MaNCC { get; set; }
        public Nullable<int> MaPhanLoai { get; set; }
        public Nullable<int> MaKM { get; set; }
    
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }
        public virtual ICollection<GioHang> GioHangs { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual KhuyenMai KhuyenMai { get; set; }
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual PhanLoaiSP PhanLoaiSP { get; set; }
    }
}
