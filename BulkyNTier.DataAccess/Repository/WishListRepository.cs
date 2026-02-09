using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class WishListRepository : Repository<WishListItem>, IWishListRepository
    {
        private readonly AppDbContext _appDbContext;
        public WishListRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        void Update(WishListItem wishList)
        {
           _appDbContext.WishList.Update(wishList);
        }
    }
}
