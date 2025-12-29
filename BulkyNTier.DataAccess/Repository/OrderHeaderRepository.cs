using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class OrderHeaderRepository:Repository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly AppDbContext _appDbContext;
        public OrderHeaderRepository(AppDbContext appDbContext):base(appDbContext) { 
            _appDbContext= appDbContext;
        }

        public void Update(OrderHeader orderHeader)
        {
            _appDbContext.OrderHeaders.Update(orderHeader);
        }


    }
}
