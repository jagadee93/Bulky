using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyNTier.ViewComponents
{
    public class CartTableViewComponent : ViewComponent
    {

        private readonly IUnitOfWork _unitOfWork;
        public CartTableViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

     


        public async Task<IViewComponentResult> InvokeAsync()
        {
            CartTableVM cartTableVM = new CartTableVM
            {
                ShoppingCartList = [],
                OrderTotal = 0

            };
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (userId != null)
            {
                cartTableVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
                Functions functions = new Functions();
                foreach (var cart in cartTableVM.ShoppingCartList)
                {

                    cart.Price = functions.GetPriceBasedOnQuantity(cart);
                    cartTableVM.OrderTotal += (decimal)cart.Price * cart.Count;

                }

                return View(cartTableVM);
            }
           
            return View(cartTableVM);

        }
    }
}