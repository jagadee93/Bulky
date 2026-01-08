using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using System.Diagnostics;

namespace BulkyNTier.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    public class OrderController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderHeader OrderHeader { get; set; }


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }




        public IActionResult Index()
        {
          

            return View();
        }



        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser,ShippingAddress");
            if (orderHeader == null)
            {
                return NotFound();
            }
            orderHeader.OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderHeader.Id, includeProperties: "Product").ToList() ?? [];
           return View(orderHeader);
        }


        [HttpPost]
        [Authorize(Roles=SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == OrderHeader.Id, includeProperties: null);
            if (orderHeaderFromDb == null)
            {
                return NotFound();

            }
            if (!String.IsNullOrEmpty(OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderHeader.TrackingNumber;
            }

            if (!String.IsNullOrEmpty(OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderHeader.Carrier;
            }
            if (orderHeaderFromDb.PaymentDueDate < OrderHeader.PaymentDueDate|| orderHeaderFromDb.PaymentDueDate==null && orderHeaderFromDb.PaymentStatus==PaymentStatus.ApprovedForDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = OrderHeader.PaymentDueDate;
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order details updated successfully";

            return RedirectToAction("Details", new { id = orderHeaderFromDb.Id });
        }



        [HttpPost]
        public IActionResult Details(OrderHeader orderHeader)
        {
            return View(orderHeader);
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
