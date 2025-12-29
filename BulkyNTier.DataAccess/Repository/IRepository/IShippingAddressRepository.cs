using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository.IRepository
{
    public interface IShippingAddressRepository:IRepository<ShippingAddress>
    {
        void Update(ShippingAddress shippingAddress);
    }
}
