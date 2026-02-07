using BulkyNTier.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BulkyNTier.Utilities;
using BulkyNTier.Models;
namespace BulkyNTier.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            if (id != null)
            {
                var company = _unitOfWork.CompanyRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);
                return View(company);
            }





            return View(new Company());
        }


        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                //Update or Create
                if (company.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(company);
                    TempData["success"] = "company has been Created";
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(company);
                    TempData["success"] = "company has been updated";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }




        #region
        public IActionResult GetAll()
        {
           IEnumerable<Company> companies = _unitOfWork.CompanyRepository.GetAll(includeProperties: null).ToList();
            return Json(companies);
        }




        [HttpDelete]
        public IActionResult Delete(int? id)
        {


            var Company = _unitOfWork.CompanyRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: null);
            if (Company != null)
            {
                _unitOfWork.CompanyRepository.Remove(Company);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Company not found" });
            }

            return Json(new { success = true, message = "Company not found" });
        }



        #endregion
    }
}
