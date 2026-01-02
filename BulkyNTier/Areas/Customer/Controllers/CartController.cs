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

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public int shippingAddressId { get; set; }

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
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
        public IActionResult InitiateCheckout(ShoppingCartListVM shoppingCartListVM)
        {
            TempData["SelectedAddressId"] = shoppingCartListVM.SelectedShippingAddressId;
            return RedirectToAction(nameof(Checkout), "Cart");
        }



        [HttpPost]
        public IActionResult InitiateOrder(ShoppingCartListVM shoppingCartListVM)
        {
            return RedirectToAction(nameof(Payment));
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



        public IActionResult Payment()
        {
            return View();
        }



        [HttpGet]
        public IActionResult GetPriceDetailsComponent()
        {
            return ViewComponent(nameof(PriceDetailsViewComponent));
        }



        [HttpGet]
        public IActionResult ShoppingCartCount()
        {
            return ViewComponent(nameof(CartCountViewComponent));
        }



    }
}
