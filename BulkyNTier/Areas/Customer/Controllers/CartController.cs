using BulkyNTier.DataAccess.Repository;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ShoppingCartListVM shoppingCartListVM = new() { };

                IEnumerable<ShoppingCart> carts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == user.Id, includeProperties: "Product");

                foreach (var cart in carts)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    shoppingCartListVM.Total += cart.Price * cart.Count;

                }
                shoppingCartListVM.ShoppingCartList = carts;


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
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");

        }



        public double GetPriceBasedOnQuantity(ShoppingCart cart)
        {


            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }

        }

    }
}
