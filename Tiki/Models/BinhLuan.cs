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

    public partial class BinhLuan
    {
        public int MaBinhLuan { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<int> MaSP { get; set; }

        [Required(ErrorMessage ="N?i dung kh�ng ???c ?? tr?ng !")]
        public string NoiDung { get; set; }
        public Nullable<System.DateTime> NgayBinhLuan { get; set; }
    
        public virtual KhachHang KhachHang { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
