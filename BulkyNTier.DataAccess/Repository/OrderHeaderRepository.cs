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

        public void UpdateStatus(int id, OrderStatus orderStatus, PaymentStatus paymentStatus = PaymentStatus.Pending)
        {

            var orderFromDb= _appDbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if(paymentStatus != PaymentStatus.Pending)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
            
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _appDbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);

            if (!string.IsNullOrEmpty(sessionId))   
            {
                orderFromDb.SessionId=sessionId;

            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;

            }
        }
    }
}
