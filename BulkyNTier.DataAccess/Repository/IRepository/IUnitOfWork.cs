using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {

         ICategoryRepository CategoryRepository { get; set; }
         IProductRepository ProductRepository { get; set; }
         ICompanyRepository CompanyRepository { get; set; }
         IShoppingCartRepository ShoppingCartRepository { get; set; }

         IOrderHeaderRepository OrderHeaderRepository { get; set; }

         IOrderDetailRepository OrderDetailRepository { get; set; }

         IOrderEventRepository OrderEventRepository { get; set; }

         IWishListRepository WishListRepository { get; set; }

         IApplicationUserRepository ApplicationUserRepository { get; set; }

         IShippingAddressRepository ShippingAddressRepository { get; set; }

         IProductImageRepository ProductImageRepository { get; set; }

        void Save();
    }
}
