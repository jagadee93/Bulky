using BulkyNTier.DataAccess.Repository;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using BulkyNTier.Utilities;
using BulkyNTier.ViewComponents;
using Newtonsoft.Json;
using Stripe.Checkout;
using Microsoft.Extensions.Options;

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly StripeSettings _stripeSettings;


        public int ShippingAddressId { get; set; }

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,IOptions<StripeSettings> stripeSettingsAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _stripeSettings = stripeSettingsAccessor.Value;
        }


        public async Task<IActionResult> Index()
        {

            var claimsIdentity =  User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (userId != null)
            {
                IEnumerable<ShippingAddress> shippingAddresses =  _unitOfWork.ShippingAddressRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: null).OrderByDescending(u => u.IsDefaultAddress);
                if (shippingAddresses == null)
                {
                    shippingAddresses = new List<ShippingAddress>();

                }
                ShoppingCartListVM shoppingCartListVM = new()
                {
                    OrderHeader = new(),
                    ShippingAddresses = shippingAddresses,
                    NewShippingAddress = new()

                };

                IEnumerable<ShoppingCart> carts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
                Functions Helpers = new Functions();
                foreach (var cart in carts)
                {
                   
                    cart.Price = Helpers.GetPriceBasedOnQuantity(cart);
                    shoppingCartListVM.OrderHeader.OrderTotal += (decimal)cart.Price * cart.Count;

                }
                shoppingCartListVM.ShoppingCartList = carts;
               


                //Determine the default address to highlight 
                shoppingCartListVM.SelectedShippingAddressId =
                 shippingAddresses.FirstOrDefault(a => a.IsDefaultAddress)?.Id
                ?? shippingAddresses.FirstOrDefault()?.Id;

                return View(shoppingCartListVM);
            }

            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }


        public IActionResult RemoveSingleItemFromCart(int CartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(u => u.Id == CartId, includeProperties: null);
            if (cart != null)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cart);
                _unitOfWork.Save();
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return ViewComponent("CartTable");
                }
                return RedirectToAction("Index");
            }
           
            return RedirectToAction("Index");

        }

        public IActionResult Plus(int CartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(u => u.Id == CartId, includeProperties: null);
            if (cart != null)
            {
                cart.Count += 1;
                _unitOfWork.ShoppingCartRepository.Update(cart);
                _unitOfWork.Save();
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return ViewComponent("CartTable");
                }
                return RedirectToAction("Index");

            }
           

            return RedirectToAction("Index");

        }

        public IActionResult Minus(int CartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(u => u.Id == CartId, includeProperties: null);
            if (cart != null)
            {
               
                if (cart.Count <=1)
                {
                    _unitOfWork.ShoppingCartRepository.Remove(cart);
                }
                else
                {
                    cart.Count -= 1;
                    _unitOfWork.ShoppingCartRepository.Update(cart);

                }

                _unitOfWork.Save();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return ViewComponent("CartTable");
                }
                return RedirectToAction("Index");

            }
 
            return RedirectToAction("Index");

        }


        public IActionResult Create(ShoppingCartListVM shoppingCartVM)
        {
            var address = shoppingCartVM.NewShippingAddress;
            
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
            
            if (userId == null)
            {
                return RedirectToAction(nameof(Index), "Cart");
            }
            address.ApplicationUserId = userId;

            if (ModelState.IsValid) {
                if (address.IsDefaultAddress)
                {
                    var PreviousDefaultAddress = _unitOfWork.ShippingAddressRepository.GetFirstOrDefault(u => u.ApplicationUserId == userId && u.IsDefaultAddress, includeProperties: null);
                    if (PreviousDefaultAddress != null)
                    {
                        PreviousDefaultAddress.IsDefaultAddress = false;
                        _unitOfWork.ShippingAddressRepository.Update(PreviousDefaultAddress);
                    }
                }
                _unitOfWork.ShippingAddressRepository.Add(address);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index),"Cart");
            }
            return RedirectToAction(nameof(Index), "Cart");
        }



        [HttpPost]
        public IActionResult AddAddressAjax([FromBody] ShippingAddress address)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (userId == null)
            {
                return RedirectToAction(nameof(Index), "Cart");
            }
            address.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {
                // Handle logic to unset previous default if this is new default
                if (address.IsDefaultAddress)
                {
                    var PreviousDefaultAddress = _unitOfWork.ShippingAddressRepository.GetFirstOrDefault(u => u.ApplicationUserId == userId && u.IsDefaultAddress, includeProperties: null);
                    if (PreviousDefaultAddress != null)
                    {
                        PreviousDefaultAddress.IsDefaultAddress = false;
                        _unitOfWork.ShippingAddressRepository.Update(PreviousDefaultAddress);
                    }
                }
                _unitOfWork.ShippingAddressRepository.Add(address);
                _unitOfWork.Save();

                return Json(new { success = true, addressId = address.Id, addressName = address.Name });
            }
            return Json(new { success = false, message = "Invalid Data" });
        }




        [HttpPost]
        public IActionResult InitiateCheckout(ShoppingCartListVM shoppingCartListVM)
        {
            TempData["SelectedAddressId"] = shoppingCartListVM.SelectedShippingAddressId;
            return RedirectToAction(nameof(Checkout), "Cart");
        }


            
        [HttpPost]
        public async Task<IActionResult> InitiateOrder(ShoppingCartListVM shoppingCartListVM)
        {
            var AddressId = shoppingCartListVM.SelectedShippingAddressId;
            if (AddressId == 0|| AddressId==null)
            {
                return View(shoppingCartListVM);
            }


            var orderHeader=new OrderHeader();
            var user=await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            orderHeader.ApplicationUserId = user.Id;
            orderHeader.ShippingAddressId = (int)AddressId;
          
            orderHeader.OrderDate= DateTime.Now;
            orderHeader.CreatedAt = DateTime.Now;
            

            //Determine companyUser or not 
            
           


            IEnumerable<ShoppingCart> carts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == user.Id, includeProperties: "Product");
            Functions Helpers = new ();
            foreach (var cart in carts)
            {

                cart.Price = Helpers.GetPriceBasedOnQuantity(cart);
                orderHeader.OrderTotal+= (decimal)cart.Price * cart.Count;

            }

            bool isCompanyUser = user.CompanyId != 0&& user.CompanyId != null;
            if (isCompanyUser)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var after30Days = today.AddDays(30);
                orderHeader.OrderStatus = OrderStatus.Approved;
                orderHeader.PaymentStatus = PaymentStatus.ApprovedForDelayedPayment;
                orderHeader.PaymentDueDate = after30Days;

            }
            else
            {
                orderHeader.OrderStatus = OrderStatus.Pending;
                orderHeader.PaymentStatus = PaymentStatus.Pending;
              
            }

            _unitOfWork.OrderHeaderRepository.Add(orderHeader);
            _unitOfWork.Save();

            foreach(var cart in carts)
            {
                OrderDetail detail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId=orderHeader.Id, //we have saved the order Header so it will be populated with Id
                    Count = cart.Count,
                    Price = cart.Price,
                };
                _unitOfWork.OrderDetailRepository.Add(detail);
               
            }
            _unitOfWork.Save();
            if (isCompanyUser)
            {
                _unitOfWork.ShoppingCartRepository.RemoveRange(carts);
                _unitOfWork.Save();
                return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeader.Id });
            }


            var domain = _stripeSettings.AppURL;
            if (String.IsNullOrEmpty(domain))
            {
                throw new Exception("Appsettings json is not set");
            }
            var options = new Stripe.Checkout.SessionCreateOptions
            {

                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={orderHeader.Id}",
                CancelUrl = domain + "Customer/Cart/Index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };
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

            //return RedirectToAction(nameof(Payment));
        }


        public IActionResult OrderConfirmation(int id)
        {

            OrderHeader orderHeader= _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: "ShippingAddress");
            if (orderHeader.PaymentStatus != PaymentStatus.ApprovedForDelayedPayment)
            {
                 //this is an order by customer
                 var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    //update the order status
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(id, OrderStatus.Approved, PaymentStatus.Approved);
                    _unitOfWork.Save();
                    orderHeader.PaymentStatus=PaymentStatus.Approved;
                    orderHeader.OrderStatus=OrderStatus.Approved;
                }
                
            }
            //clear the shopping cart
            IEnumerable<ShoppingCart> carts=_unitOfWork.ShoppingCartRepository.GetAll(u=>u.ApplicationUserId==orderHeader.ApplicationUserId,includeProperties:null).ToList();
            _unitOfWork.ShoppingCartRepository.RemoveRange(carts);
            _unitOfWork.Save();
            return View(orderHeader);

        }







        [HttpGet]
        public IActionResult Checkout()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (userId != null)
            {
                IEnumerable<ShippingAddress> shippingAddresses = _unitOfWork.ShippingAddressRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: null).OrderByDescending(u => u.IsDefaultAddress);
                if (shippingAddresses == null)
                {
                    shippingAddresses = new List<ShippingAddress>();

                }
                ShoppingCartListVM shoppingCartListVM = new()
                {
                    OrderHeader = new(),
                    ShippingAddresses = shippingAddresses,
                    NewShippingAddress = new()

                };

                IEnumerable<ShoppingCart> carts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

                foreach (var cart in carts)
                {
                    Functions Helpers = new Functions();
                    cart.Price = Helpers.GetPriceBasedOnQuantity(cart);
                    shoppingCartListVM.OrderHeader.OrderTotal += (decimal)cart.Price * cart.Count;

                }
                shoppingCartListVM.ShoppingCartList = carts;



                //Determine the default address to highlight 
                shoppingCartListVM.SelectedShippingAddressId =
                 shippingAddresses.FirstOrDefault(a => a.IsDefaultAddress)?.Id
                ?? shippingAddresses.FirstOrDefault()?.Id;

                return View(shoppingCartListVM);
            }

            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }



        public IActionResult Payment(OrderHeader orderHeader)
        {
            return View();
        }



        [HttpGet]
        public IActionResult GetPriceDetailsComponent()
        {
            return ViewComponent("PriceDetails");
        }



        [HttpGet]
        public IActionResult GetShoppingCartCount()
        {
            return ViewComponent("CartCount");
        }

        [HttpGet]
        public IActionResult GetCartTableComponent(bool isReadOnly = false)
        {
            return ViewComponent("CartTable");
        }


        [HttpPost]
        public IActionResult AddAddress([FromBody] ShippingAddress address)
        {


            Console.WriteLine(address);
            // If binding fails due to Enum or Required fields, address is null
            if (address == null)
            {
                return Json(new { success = false, message = "Could not parse address data. Check Enum values." });
            }

            

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                address.ApplicationUserId = userId;
            }
            // VERY IMPORTANT: 
            // Navigation properties must be removed from validation since they aren't in the JSON
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("ApplicationUserId"); // Because we set it manually after binding

            if (ModelState.IsValid)
            {
                if (address.IsDefaultAddress)
                {
                    var oldDefault = _unitOfWork.ShippingAddressRepository.GetFirstOrDefault(
                        u => u.ApplicationUserId == userId && u.IsDefaultAddress,includeProperties:null);
                    if (oldDefault != null)
                    {
                        oldDefault.IsDefaultAddress = false;
                        _unitOfWork.ShippingAddressRepository.Update(oldDefault);
                    }
                }

                _unitOfWork.ShippingAddressRepository.Add(address);
                _unitOfWork.Save();
                return Json(new { success = true });
            }

            // If we reach here, validation failed. Let's see why:
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(" | ", errors) });
        }



        [HttpGet]
        public IActionResult GetAddressList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //we dont need to sort by Default address here this after creating new address
            var addresses = _unitOfWork.ShippingAddressRepository
                .GetAll(u => u.ApplicationUserId == userId, includeProperties: null)
                .OrderByDescending(u => u.Id);

            return PartialView("_AddressList", addresses);
        }
    }
}
