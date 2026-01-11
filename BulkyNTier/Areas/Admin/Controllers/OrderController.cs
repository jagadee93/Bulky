using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyNTier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _stripeSettings;
        [BindProperty]
        public OrderHeader OrderHeader { get; set; }


        public OrderController(IUnitOfWork unitOfWork, IOptions<StripeSettings> stripeSettingoptions)
        {
            _unitOfWork = unitOfWork;
            _stripeSettings = stripeSettingoptions.Value;
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

            orderHeaderFromDb.Carrier = orderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = orderHeader.TrackingNumber;
            if (orderHeaderFromDb.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(30);
            }
            orderHeaderFromDb.OrderStatus = OrderStatus.Shipped;

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

                var service = new RefundService();

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

        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_Pay_Now(OrderHeader orderHeader)
        {
            OrderHeader orderHeaderFromDB = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == orderHeader.Id, includeProperties:null);


            if (orderHeaderFromDB == null)
            {
                return NotFound();
            }

            var domain = _stripeSettings.AppURL;

            if (String.IsNullOrEmpty(domain))
            {
                throw new Exception("Appsettings json is not set");
            }
            var options = new Stripe.Checkout.SessionCreateOptions
            {

                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderId={orderHeaderFromDB.Id}",
                CancelUrl = domain + $"Admin/Order/Details?id={orderHeaderFromDB.Id}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };
            var carts = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderHeaderFromDB.Id, includeProperties: "Product").ToList() ?? [];
            foreach (var cartItem in carts)
            {
                var SessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cartItem.Price * 100),//20.50 =>2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Product.Title
                        }
                    },
                    Quantity = cartItem.Count,
                };

                options.LineItems.Add(SessionLineItem);
            }
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);



            //PaymentIntentId will be populated in session only after payment is successfull

            _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);

            _unitOfWork.Save();


            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);


        }


        public IActionResult PaymentConfirmation(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ShippingAddress");
            if (orderHeader == null)
            {
                return NotFound();
            }

            if (orderHeader.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment)
            {
                //retrive the session
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                    //Update Payment Status
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, orderHeader.OrderStatus, PaymentStatus.Approved);
                    _unitOfWork.Save();

                    orderHeader.PaymentStatus = PaymentStatus.Approved;

                }
            }
            return View(orderHeader);
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
