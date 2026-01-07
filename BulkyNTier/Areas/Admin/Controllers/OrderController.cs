using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyNTier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;



        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }




        public IActionResult Index()
        {
          

            return View();
        }


        #region API CALLS 
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> OrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser,ShippingAddress") ?? new List<OrderHeader>();
    

                switch (status)
            {
                case "pending":
                    OrderHeaders= OrderHeaders.Where(u=>u.PaymentStatus==PaymentStatus.ApprovedForDelayedPayment).ToList();
                    break;
                case "completed":
                    OrderHeaders= OrderHeaders.Where(u => u.OrderStatus ==OrderStatus.Delivered ).ToList();
                    break;
                case "approved":
                    OrderHeaders=OrderHeaders.Where(u=>u.OrderStatus==OrderStatus.Approved).ToList();
                    break;
                case "inprocess":
                    OrderHeaders = OrderHeaders.Where(u => u.OrderStatus == OrderStatus.Processing).ToList();
                    break;
                default:
                    break;
            }




            return new JsonResult(OrderHeaders);
        }

        #endregion
    }
}
