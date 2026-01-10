using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyNTier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderHeader OrderHeader { get; set; }


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
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
            if (orderHeaderFromDb.PaymentDueDate < OrderHeader.PaymentDueDate || orderHeaderFromDb.PaymentDueDate == null && orderHeaderFromDb.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = OrderHeader.PaymentDueDate;
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order details updated successfully";

            return RedirectToAction("Details", new { id = orderHeaderFromDb.Id });
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult ProcessOrder(OrderHeader orderHeader)
        {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderHeader.Id, OrderStatus.Processing);
            _unitOfWork.Save();
            TempData["success"] = "Order details updated successfully";
            return RedirectToAction("Details", new { id = orderHeader.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder(OrderHeader orderHeader)
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == OrderHeader.Id, includeProperties: null);

            orderHeaderFromDb.Carrier=orderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber=orderHeader.TrackingNumber;
            if (orderHeaderFromDb.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(30);
            }
            orderHeaderFromDb.OrderStatus=OrderStatus.Shipped;

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order shipped successfully";
            return RedirectToAction("Details", new { id = orderHeader.Id });
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult Cancel(OrderHeader orderHeader)
        {
            var OrderHeaderFromDb = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == orderHeader.Id, includeProperties: null);

            if (OrderHeaderFromDb.PaymentStatus == PaymentStatus.Approved && !string.IsNullOrEmpty(OrderHeaderFromDb.PaymentIntentId))
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderHeaderFromDb.PaymentIntentId
                };

                var service =new RefundService();

                Refund refund = service.Create(options);

                _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderHeaderFromDb.Id, OrderStatus.Cancelled, PaymentStatus.Refunded);
            }
            else
            {
                _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderHeaderFromDb.Id, OrderStatus.Cancelled, PaymentStatus.Cancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order cancelled successfully";
            return RedirectToAction("Details", new { id = orderHeader.Id });
        }




        #region API CALLS 
        public IActionResult GetAll(string status)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> OrderHeaders = new List<OrderHeader>();

            //If Admin or Employee retrive all orders
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser,ShippingAddress") ?? new List<OrderHeader>();
            }
            else
            {
                OrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser,ShippingAddress");
            }

                

            switch (status)
            {
                case "pending":
                    OrderHeaders = OrderHeaders.Where(u => u.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment).ToList();
                    break;
                case "completed":
                    OrderHeaders = OrderHeaders.Where(u => u.OrderStatus == OrderStatus.Delivered).ToList();
                    break;
                case "approved":
                    OrderHeaders = OrderHeaders.Where(u => u.OrderStatus == OrderStatus.Approved).ToList();
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
