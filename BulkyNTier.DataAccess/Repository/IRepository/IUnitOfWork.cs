using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }

        IShippingAddressRepository ShippingAddressRepository {  get;}
        IOrderHeaderRepository  OrderHeaderRepository { get; }
        IOrderDetailRepository OrderDetailRepository {  get; }
        IOrderEventRepository  OrderEventRepository { get; }

        IWishListRepository WishListRepository { get; }

        void Save();
    }
}
