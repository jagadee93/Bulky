using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyNTier.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(0);
            }
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;


            if (userId!=null)
            {
                var ShoppingCartCount = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: null).Sum(u => u.Count);
                return View(ShoppingCartCount);
            }

            return View(0);

        }
    }
}
