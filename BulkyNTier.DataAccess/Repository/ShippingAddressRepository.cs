using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class ShippingAddressRepository : Repository<ShippingAddress>,IShippingAddressRepository
    {
        private readonly AppDbContext _appDbContext;

        public ShippingAddressRepository(AppDbContext appDbContext):base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(ShippingAddress shippingAddress)
        {
            _appDbContext.ShippingAddresses.Update(shippingAddress);
        }
    }
}
