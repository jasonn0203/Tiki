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
    
    public partial class KhuyenMai
    {
        public KhuyenMai()
        {
            this.SanPhams = new HashSet<SanPham>();
        }
    
        public int MaKM { get; set; }
        public string TenKM { get; set; }
        public decimal UuDai { get; set; }
        public Nullable<int> MaNCC { get; set; }
    
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}