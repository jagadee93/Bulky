using BulkyNTier.DataAccess;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyNTier.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        public readonly IUnitOfWork unitOfWork ;
        public CategoryController(IUnitOfWork db)
        {
             unitOfWork= db;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = unitOfWork.CategoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }


        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Create(Category obj)
        {

            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "name and DisplayOrder should not be same..");
            //}

            //if (obj.Name == "test")
            //{
            //    ModelState.AddModelError("", "Name is invalid");
            //}

            if (ModelState.IsValid)
            {
                unitOfWork.CategoryRepository.Add(obj);
                unitOfWork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();
        }


        public IActionResult Edit(int? Id)
        {
            Console.WriteLine(Id);


            Category? obj = unitOfWork.CategoryRepository.GetFirstOrDefault(u => u.Id == Id);
            if (obj == null || Id == 0 || Id == null)
            {
                return NotFound();
            }
            Console.WriteLine(obj);


            return View(obj);
        }

        [HttpPost]
        public IActionResult Edit(Category updatedObj)
        {

            if (updatedObj == null)
            {
                return NotFound();

            }

            Console.WriteLine(updatedObj);
            if (ModelState.IsValid)
            {
                unitOfWork.CategoryRepository.Update(updatedObj);
                unitOfWork.Save();
                TempData["success"] = "Category edited Successfully";
            }

            return RedirectToAction("Index");



        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? objTobeDeleted = unitOfWork.CategoryRepository.GetFirstOrDefault(u => u.Id == id);

            if (objTobeDeleted == null)
            {
                TempData["error"] = $"Category with {id} could not be found";
            }

            unitOfWork.CategoryRepository.Remove(objTobeDeleted);
            unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");
        }
    }

}
