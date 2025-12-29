using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
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


        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork,SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork=unitOfWork;
            _logger = logger;
            _signInManager=signInManager;
            _userManager=userManager;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(ShoppingCart shoppingCart)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
            if (userId == null)
            {
            
                return RedirectToPage("/Account/Login", new { area = "Identity" });
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
            TempData["success"] = cartFromDb!=null ? "Cart Updated Successfully" : "Added to Cart..";
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
