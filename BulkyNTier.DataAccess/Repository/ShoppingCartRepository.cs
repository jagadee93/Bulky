using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;

namespace BulkyNTier.DataAccess.Repository
{
    public class ShoppingCartRepository:Repository<ShoppingCart>,IShoppingCartRepository
    {
        private readonly AppDbContext _appDbContext;
        

        public ShoppingCartRepository(AppDbContext appDbContext):base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _appDbContext.ShoppingCarts.Update(shoppingCart);
        }

    }
}
