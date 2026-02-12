using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Models.ViewModels;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace BulkyNTier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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

            IEnumerable<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return View(products);
        }




        //Handles Create and Edit 
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll(includeProperties: null)
                .Select(u => new SelectListItem
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
                productVM.Product = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: "ProductImages");

                return View(productVM);

            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (productVM.Product.Id == 0)
            {
                //Add Default Image...
                //if (file == null)
                //{
                //    productVM.Product.ImageURL =" ";

                //}
                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Save();

            }


            if (ModelState.IsValid)
            {

                string productPath = @"images/products/product-" + $"{productVM.Product.Id}";
                string productImgPath = Path.Combine(wwwRootPath, productPath);
                if (!Directory.Exists(productImgPath))
                {
                    Directory.CreateDirectory(productImgPath);
                }
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file?.FileName);
                        using (var fileStream = new FileStream(Path.Combine(productImgPath, fileName), FileMode.Create))
                        {
                            file?.CopyTo(fileStream);
                        }


                        var productImage = new ProductImage()
                        {
                            ProductId = productVM.Product.Id,
                            ImageURL = $"/{productPath}/" + fileName,
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = [];

                        }

                        productVM.Product.ProductImages.Add(productImage);
                        //save productImage
                        // _unitOfWork.ProductImageRepository.Add(productImage);
                    }
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product has been updated";
                return RedirectToAction("Index");
            }
            return View(productVM);
        }




        public IActionResult DeleteImage(int? id)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            var ImageToBeDeleted = _unitOfWork.ProductImageRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);

            if (ImageToBeDeleted == null)
            {
                TempData["error"] = "Image not found..";
                return RedirectToAction(nameof(Index));
            }
            int productId = ImageToBeDeleted.ProductId;

            var relativePath = ImageToBeDeleted.ImageURL
                .TrimStart('/', '\\'); // Handles both slash types
            string imgFilePath = Path.Combine(wwwRootPath,relativePath);
            if (System.IO.File.Exists(imgFilePath))
            {
                System.IO.File.Delete(imgFilePath);
            }

            _unitOfWork.ProductImageRepository.Remove(ImageToBeDeleted);
            _unitOfWork.Save();
            TempData["success"] = "Image deleted successfully..";
            return RedirectToAction(nameof(Upsert), new { id = ImageToBeDeleted.ProductId });

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


        //public IActionResult Delete(int? id)
        //{
        //    var productToBeDeleted = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);
        //    if (productToBeDeleted == null)
        //    {
        //        TempData["error"] = "Product not found";
        //        return NotFound();
        //    }


        //    if (!string.IsNullOrEmpty(productToBeDeleted.ImageURL))
        //    {
        //        //delete the old image
        //        //Delete the forword slash
        //        string wwwRootPath = _webHostEnvironment.WebRootPath;
        //        string imgFilePath = Path.Combine(wwwRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));
        //        if (System.IO.File.Exists(imgFilePath))
        //        {
        //            System.IO.File.Delete(imgFilePath);
        //        }
        //    }
        //    _unitOfWork.ProductRepository.Remove(productToBeDeleted);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product has been deleted";
        //    return RedirectToAction("Index");
        //}




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products1 = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return Json(products1);
        }





        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);

            if (productToBeDeleted == null)
            {
                return Json(new { success = "false", message = "Product not found" });
            }

            string productPath = @"images/products/product-" + id;
            string productImgPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            //delete every file in dir before dir deletion
            if (Directory.Exists(productImgPath))
            {
                string[] filePaths = Directory.GetFiles(productImgPath);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    System.IO.File.Delete(filePaths[i]);
                }
                Directory.Delete(productImgPath);
            }
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = "true", message = "Product has been deleted" });

        }

        #endregion

    }
}






