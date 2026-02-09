using BulkyNTier.DataAccess;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;


        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork,SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> userManager,AppDbContext db)
        {
            _unitOfWork=unitOfWork;
            _logger = logger;
            _signInManager=signInManager;
            _userManager=userManager;
            _db=db;
        }

        public IActionResult Index()
        {
           // List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category,ProductImage").ToList();


            IEnumerable<ProductCardVM> products = _db.Products.Select(p => new ProductCardVM
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Author= p.Author,
                DiscoutPercentage=(int) ((1 - (p.Price / p.ListPrice)) * 100),
                ListPrice= p.ListPrice,
                ImageURL = p.ProductImages
                            .Select(i => i.ImageURL)
                            .FirstOrDefault()

            });
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(ShoppingCart? shoppingCartfromModel=null, int? productId=null,bool wishList=false)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
            if (userId == null)
            {
             return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            ShoppingCart shoppingCart = new();

            if (!wishList)
            {
                shoppingCart = shoppingCartfromModel;
            }


            if (productId != null&&wishList)
            {
                var productFromDb = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == productId,includeProperties:null);
                if (productFromDb != null)
                {
                    shoppingCart.ProductId = productFromDb.Id;
                    shoppingCart.Price = productFromDb.Price;
                    shoppingCart.Count = 1;
                   
                }
                
            }

            shoppingCart.ApplicationUserId = userId;

            //check if the Product already Exists
            var cartFromDb = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(u =>
              u.ApplicationUserId == userId &&
              u.ProductId == shoppingCart.ProductId,includeProperties:null);
            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);

            }
            else
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            }
            _unitOfWork.Save();

            if (wishList&&productId!=null)
            {
               var ExistingWishListItem= _unitOfWork.WishListRepository.GetFirstOrDefault(u=>u.ProductId==productId,includeProperties:null);
                if (ExistingWishListItem!=null)
                {
                    _unitOfWork.WishListRepository.Remove(ExistingWishListItem);
                    _unitOfWork.Save();
                }

                return Json(new { success = true,message="Product moved to cart" });
               
            }
            TempData["success"] = cartFromDb != null ? "Cart Updated Successfully" : "Added to Cart..";
           

            return RedirectToAction("Details", new { id = shoppingCart.ProductId });
        }



        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category"),
                ProductId = id,
                Count = 1
            };
            return View(shoppingCart);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
