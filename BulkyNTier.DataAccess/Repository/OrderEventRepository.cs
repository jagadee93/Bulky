using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class OrderEventRepository:Repository<OrderEvent>,IOrderEventRepository
    {
        private readonly AppDbContext _appDbContext;

        public OrderEventRepository(AppDbContext appDbContext):base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(OrderEvent orderEvent)
        {
            _appDbContext.OrderEvents.Update(orderEvent);
        }
    }
}
