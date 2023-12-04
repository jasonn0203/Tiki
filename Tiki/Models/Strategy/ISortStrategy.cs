using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiki.Models.Strategy
{
    public interface ISortStrategy
    {
        List<SanPham> SortProducts(List<SanPham> products);
    }


    public class PriceAscendingSortingStrategy : ISortStrategy
    {
        public List<SanPham> SortProducts(List<SanPham> products)
        {
            return products.OrderBy(product => product.Gia).ToList();
        }
    }

    public class PriceDescendingSortingStrategy : ISortStrategy
    {
        public  List<SanPham> SortProducts(List<SanPham> products)
        {
            return products.OrderByDescending(product => product.Gia).ToList();
        }
    }

    public class DefaultSortingStrategy : ISortStrategy
    {
        public List<SanPham> SortProducts(List<SanPham> products)
        {
            return products.OrderByDescending(product => product.TenSanPham).ToList();
        }
    }

}
