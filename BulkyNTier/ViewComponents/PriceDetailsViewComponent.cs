using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using BulkyNTier.Utilities;
namespace BulkyNTier.ViewComponents
{
    public class PriceDetailsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;


        public PriceDetailsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public async Task<IViewComponentResult> InvokeAsync()
        {
            //if (!HttpContext.User.Identity.IsAuthenticated)
            //{
            //    return View(new PriceDetailSummaryVM());
            //}
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
            PriceDetailSummaryVM priceDetailSummary = new PriceDetailSummaryVM();
            IEnumerable<ShoppingCart> ShoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,includeProperties:"Product");
            Functions helperFunction=new();

            foreach (var cart in ShoppingCarts)
            {
                priceDetailSummary.TotalPrice +=(decimal) helperFunction.GetPriceBasedOnQuantity(cart)*cart.Count;
                priceDetailSummary.ListPriceTotal += cart.Count * (decimal)cart.Product.ListPrice;
            }
            decimal savings = 0;
            priceDetailSummary.ShippingCharges = 0;
            savings = priceDetailSummary.ListPriceTotal - priceDetailSummary.TotalPrice;
            priceDetailSummary.TotalSavings = savings;
            priceDetailSummary.DiscountTotal = savings;
            priceDetailSummary.CouponDiscount = 0;
            return View(priceDetailSummary);
        }
    }
}
