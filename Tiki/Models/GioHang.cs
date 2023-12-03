using System.Linq;

namespace Tiki.Models
{
    public class GioHang
    {
        TikiEntities db = new TikiEntities();

        public int MaSP { get; set; }
        public int MaKH { get; set; }
        public string TenSanPham { get; set; }

        public string HinhAnh1 { get; set; }

        public decimal Gia { get; set; }
        public short SoLuong { get; set; }

        public decimal TongTien
        {
            get { return Gia * SoLuong; }
        }

        public GioHang(int maSP, short soLuong)
        {
            MaSP = maSP;
            SanPham sp = db.SanPhams.Single(s => s.MaSP == maSP);
            TenSanPham = sp.TenSanPham;
            HinhAnh1 = sp.HinhAnh1;
            Gia = (decimal)sp.Gia;
            SoLuong = soLuong;
        }


    }
}