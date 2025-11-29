using System.Diagnostics;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
            _logger = logger;
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

        public IActionResult Details(int? id)
        {
            Product product = _unitOfWork.ProductRepository.GetFirstOrDefault(u=>u.Id==id,includeProperties:"Category");
            return View(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
