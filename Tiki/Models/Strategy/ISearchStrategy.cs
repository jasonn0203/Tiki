using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiki.Models.Strategy
{
    public interface ISearchStrategy
    {
        List<SanPham> Search(string searchString, IQueryable<SanPham> sanPhams);
    }


    public class TenSanPhamSearchStrategy : ISearchStrategy
    {
        public List<SanPham> Search(string searchString, IQueryable<SanPham> sanPhams)
        {
            return sanPhams.Where(s => s.TenSanPham.Contains(searchString)).ToList();
        }
    }
}
