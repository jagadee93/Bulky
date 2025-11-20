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
        private IWebHostEnvironment _webHostEnvironment;//This already Injected By Default .


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }



        public IActionResult Index()
        {

            List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
            return View(products);
        }




        //Handles Create and Edit 
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll(includeProperties:null).
                  Select(u => new SelectListItem
                  {
                      Text = u.Name,
                      Value = u.Id.ToString()
                  }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVM);

            }
            else
            {
                productVM.Product = _unitOfWork.ProductRepository.GetFirstOrDefault(u=>u.Id == id,includeProperties:null);
             
                return View(productVM);

            }
           
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)
        {

           
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)//check the file
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file?.FileName);
                    string productImgPath = Path.Combine(wwwRootPath, @"images\product");


                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL)) {
                        //delete the old image
                        //Delete the forword slash
                        string pathOfImageTobeDeleted = Path.Combine(wwwRootPath,productVM.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(pathOfImageTobeDeleted))
                        {
                            System.IO.File.Delete(pathOfImageTobeDeleted);
                        }

                    }


                    //Copy the original File to images Folder

                    using(var fileStream=new FileStream(Path.Combine(productImgPath, fileName), FileMode.Create))
                    {
                       file?.CopyTo(fileStream);
                    }

                    productVM.Product.ImageURL = @"\images\product\" + fileName;
                }





                //Determine wheather it is a create or update ??

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                    TempData["success"] = "Product has been Created";
                }
                else
                {

                    _unitOfWork.ProductRepository.Update(productVM.Product);
                    TempData["success"] = "Product has been updated";
                }

                    _unitOfWork.Save();
               
                return RedirectToAction("Index");
            }

            return View(productVM);

               
        }

        

        //[HttpPost]
        //public IActionResult Edit(Product productobjTobeUpdated)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.ProductRepository.Update(productobjTobeUpdated);
        //        _unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}


        public IActionResult Delete(int? id)
        {
            var productToBeDeleted=_unitOfWork.ProductRepository.GetFirstOrDefault(u=>u.Id == id, includeProperties: null);
            if (productToBeDeleted == null)
            {
                TempData["error"] = "Product not found";
                return NotFound();
            }
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();
            TempData["success"] = "Product has been deleted";
            return RedirectToAction("Index");
        }

    }
}






