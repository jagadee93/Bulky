using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyNTier.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {

        private IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {

           var products= _unitOfWork.ProductRepository.GetAll().ToList();
           return View(products);
        }

        public IActionResult Edit(int? id)
        {
            var product=_unitOfWork.ProductRepository.GetFirstOrDefault(u=>u.Id==id);
            if (product == null)
            {
                return View("Error");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product productobjTobeUpdated)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(productobjTobeUpdated);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            var productToBeDeleted=_unitOfWork.ProductRepository.GetFirstOrDefault(u=>u.Id == id);
            if (productToBeDeleted == null)
            {
                return NotFound();
            }
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

    }
}






