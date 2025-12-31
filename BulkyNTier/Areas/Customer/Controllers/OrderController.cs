using BulkyNTier.DataAccess.Repository;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Checkout()
        {
            return View();
        }


        //public IActionResult Checkout()
        //{

        //    var claimsIdentity = User.Identity as ClaimsIdentity;

        //    var userId = claimsIdentity?
        //        .FindFirst(ClaimTypes.NameIdentifier)?
        //        .Value;
        //    ShoppingCartListVM shoppingCartListVM = new ShoppingCartListVM();
        //    shoppingCartListVM.ShippingAddresses = _unitOfWork.GetAll(u => u.ApplicationUserId == userId, includeProperties: null).OrderByDescending(u => u.IsDefaultAddress);


        //    return View(shoppingCartListVM);
        //}



        public IActionResult Payment()
        {
            return View();
        }
    }
}
