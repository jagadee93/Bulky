using BulkyNTier.DataAccess;
using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyNTier.Controllers
{
    public class CategoryController : Controller
    {
        public readonly ICategoryRepository _categoryRepository ;
        public CategoryController(ICategoryRepository db)
        {
             _categoryRepository= db;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryRepository.GetAll().ToList();
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
                _categoryRepository.Add(obj);
                _categoryRepository.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();
        }


        public IActionResult Edit(int? Id)
        {
            Console.WriteLine(Id);


            Category? obj = _categoryRepository.GetFirstOrDefault(u => u.Id == Id);
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
                _categoryRepository.Update(updatedObj);
                _categoryRepository.Save();
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
            Category? objTobeDeleted = _categoryRepository.GetFirstOrDefault(u => u.Id == id);

            if (objTobeDeleted == null)
            {
                TempData["error"] = $"Category with {id} could not be found";
            }

            _categoryRepository.Remove(objTobeDeleted);
            _categoryRepository.Save();
            TempData["success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");
        }
    }

}
