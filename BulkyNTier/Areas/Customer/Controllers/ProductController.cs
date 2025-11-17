using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            //IEnumerable<SelectListItem> categoryList = _unitOfWork.CategoryRepository.GetAll().
            //  Select(u => new SelectListItem
            //  {
            //      Text = u.Name,
            //      Value = u.Id.ToString()
            //  });

            ////ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"]=categoryList;


            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll().
                  Select(u => new SelectListItem
                  {
                      Text = u.Name,
                      Value = u.Id.ToString()
                  }),
                Product = new Product()
            };


            //IEnumerable< SelectListItem> CategoryList= _unitOfWork.CategoryRepository.GetAll().
            //  Select(u => new SelectListItem
            //  {
            //      Text = u.Name,
            //      Value = u.Id.ToString()
            //  });



            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            //elsev 
            //{
            //    IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    });
            //    productVM.CategoryList = CategoryList;
            //    return View(productVM);
            //}

            return View();

               
        }

        public IActionResult Index()
        {

           List<Product> products= _unitOfWork.ProductRepository.GetAll().ToList();
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






