using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class OrderDetailRepository:Repository<OrderDetail>,IOrderDetailRepository
    {
        private readonly AppDbContext _appDbContext;
        public OrderDetailRepository(AppDbContext appDbContext):base(appDbContext) { 
            _appDbContext = appDbContext;
        }

        public void Update(OrderDetail orderDetail)
        {
            _appDbContext.OrderDetails.Update(orderDetail);
        }
    }
}
