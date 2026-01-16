using BulkyNTier.DataAccess.Repository;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using System.Security.Claims;


namespace BulkyNTier.Areas.Customer.Controllers
{
  
    [Area("Customer")]
    public class WishListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        #pragma warning disable IDE0290
        public WishListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
     

        public IActionResult Index()
        {
   
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //GetExistingWishList
                var ExistingWishList = _unitOfWork.WishListRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product") ?? [];
                return View(ExistingWishList);
            }
            else
            {
                //GUEST --> return Diff view 
                return RedirectToAction(nameof(SessionWishList));
            }
           
        }


        [HttpGet]
        public IActionResult SessionWishList()
        {
            List<Product> products = [];
            var sessionData = HttpContext.Session.GetString(SD.WishListSessionKey);
            List<int> wishList = sessionData == null ? [] : JsonConvert.DeserializeObject<List<int>>(sessionData);
            foreach (int id in wishList)
            {
                var productFromDb = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);
                products.Add(productFromDb);

            }
            return View(products);

        }


        [HttpPost]
        public IActionResult RemoveSessionItem(int productId)
        {
    
            var sessionData = HttpContext.Session.GetString(SD.WishListSessionKey);

            List<int> wishList = sessionData == null ? [] : JsonConvert.DeserializeObject<List<int>>(sessionData);

            if (wishList.Contains(productId))
            {
                wishList.Remove(productId);
                HttpContext.Session.SetString(SD.WishListSessionKey, JsonConvert.SerializeObject(wishList));
                return Json(new { success=true, message = "Product removed from wishlist" });
            }

            return Json(new { success = false, message = "Unable to remove from wishlist" });

        }









       [HttpDelete]
       public IActionResult RemoveListItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Item = _unitOfWork.WishListRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);
            if (Item == null)
            {
                return NotFound();
            }
            _unitOfWork.WishListRepository.Remove(Item);
            _unitOfWork.Save();
            return Ok(id);

        }




        [HttpPost]
       
        public IActionResult ToggleWishListItem(int productId=0, bool removeItem=false)
        {

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //GetExistingWishListItem
                 var ExistingWishListItem = _unitOfWork.WishListRepository.GetFirstOrDefault(u => u.ApplicationUserId == userId && u.ProductId == productId,includeProperties:"Product");

                if (ExistingWishListItem != null) {
                    _unitOfWork.WishListRepository.Remove(ExistingWishListItem);
                    _unitOfWork.Save();
                    return Json(new { success = true,message="Product removed from wish list" });
                }
                else
                {
                    if (removeItem)
                    {
                        return Json(new { success = false, message = "Cound not find the product" });
                    }
                    _unitOfWork.WishListRepository.Add(new WishList { ApplicationUserId=userId,ProductId=productId });
                    _unitOfWork.Save();
                   
                    return Json(new { success = true,message="Product added to wish list" });
                }


            }
            else
            {
                //GUEST --> Use Session 


                var sessionData = HttpContext.Session.GetString(SD.WishListSessionKey);
               
                List<int> wishList=sessionData==null ? []: JsonConvert.DeserializeObject<List<int>>(sessionData);

                if (wishList.Contains(productId))
                {
                    wishList.Remove(productId);
                    HttpContext.Session.SetString(SD.WishListSessionKey, JsonConvert.SerializeObject(wishList));
                    
                    return Json(new { success = true , message = "Product removed from wish list" });
                }
                else
                {
                    wishList.Add(productId);
                    HttpContext.Session.SetString(SD.WishListSessionKey, JsonConvert.SerializeObject(wishList));
                   
                    return Json(new { success = true , message = "Product added to wish list" });
                }
            }

            
        }
    }
}

