using BulkyNTier.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BulkyNTier.Utilities;
using BulkyNTier.Models;
using BulkyNTier.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BulkyNTier.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace BulkyNTier.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUnitOfWork unitOfWork, AppDbContext dbContext,RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _db = dbContext;
            _userManager = userManager;

        }




        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public IActionResult RoleManagement(UpdateUserRoleVM updateUserRoleVM)
        {
            var dbUser = _db.Users.Include(u=>u.Company).FirstOrDefault(u => u.Id == updateUserRoleVM.UserID);
            if (dbUser == null)
            {
                return NotFound();
            }
            if (updateUserRoleVM.Role == "Company" && dbUser.CompanyId != updateUserRoleVM.CompanyId)
            {
                
                dbUser.CompanyId = updateUserRoleVM.CompanyId;
               
                TempData["success"] = "User Company updated successfully";
            }
            else
            {
                dbUser.CompanyId = null;
           
                TempData["success"] = "User Company updated successfully";
            }

                var currentRoles = _userManager.GetRolesAsync(dbUser).GetAwaiter().GetResult();
            if (!currentRoles.Contains(updateUserRoleVM.Role))
            {
                _userManager.RemoveFromRolesAsync(dbUser, currentRoles).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(dbUser, updateUserRoleVM.Role).GetAwaiter().GetResult();
                TempData["success"] = "User role updated successfully";
            }
            _db.SaveChanges();


            return RedirectToAction(nameof(RoleManagement),new { userId=updateUserRoleVM.UserID});
        }




        public IActionResult RoleManagement(string userId)
        {
            var user = _db.Users.Include(u => u.Company).FirstOrDefault(u => u.Id == userId);
            if (user == null) { 
                return NotFound();
            }


            //Role Name
            //var RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            //var RoleName = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            var roleInfo =
                 from userRole in _db.UserRoles
                 join role in _db.Roles
                 on userRole.RoleId equals role.Id
                 where userRole.UserId == userId
                 select new { role.Id,role.Name};
            var UpdateRolesVM = new UpdateUserRoleVM()
            {
                UserID=user.Id,
                CompaniesList= _db.Companies.ToList().Select(u=>new SelectListItem
                {
                    Text=u.Name,
                    Value=u.Id.ToString()
                }),
                Role=roleInfo.Select(u=>u.Name).FirstOrDefault(),
                UserName=user.UserName??user.Email+" "+roleInfo.Select(u=>u.Name).FirstOrDefault(),
                CompanyId=user.CompanyId,
                RoleList= _roleManager.Roles.ToList().Select(u=>new SelectListItem
                {
                    Text=u.Name,
                    Value=u.Name

                })
              
            };
           
            return View(UpdateRolesVM);

        }





        #region
        public IActionResult GetAll()
        {
            //List<ApplicationUser> users = _db.Users.Include(u => u.Company).ToList();



            var userWithRoles =
                from user in _db.Users.Include(u => u.Company)
                join userRole in _db.UserRoles
                    on user.Id equals userRole.UserId into ur
                from userRole in ur.DefaultIfEmpty()
                join role in _db.Roles
                    on userRole.RoleId equals role.Id into r
                from role in r.DefaultIfEmpty()
                select new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    CompanyName = user.Company.Name ?? "NA",
                    CompanyPhoneNumber = user.Company.PhoneNumber ?? "NA",
                    Role = role != null ? role.Name : "NO Role",
                    user.LockoutEnabled
                };


            // Add Roles

            //var RolesList = _db.Roles.ToList();
            //var UserRoles = _db.UserRoles.ToList();



            //foreach (var user in users)
            //{
            //    var UserRoleId = UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            //    var RoleName = RolesList.FirstOrDefault(u => u.Id == UserRoleId).Name;
            //    user.Role = RoleName;
            //}
            return Json(userWithRoles);

            //return Json(users);
        }

        [HttpPost]
        public IActionResult SetLockout([FromBody]string userId)
        {
            var user=_db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Json(new { success = false, message = "Failed to initiate lockout" });
            //true
            if (!user.LockoutEnabled)
            {
                user.LockoutEnd = DateTime.Now;
                user.LockoutEnabled = true;
                _db.SaveChanges();
                return Json(new { success=true,message="User Lockout disabled" });
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddDays(30);
                user.LockoutEnabled = false;
                _db.SaveChanges();
                return Json(new { success = true, message = "User Lockout Enabled" });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {


            var user = _db.Users.FindAsync(id).GetAwaiter().GetResult();
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Json(new { success = "True", message = "User deleted" });
            }

            return Json(new { success = "False", message = "User not found" });
        }



        #endregion
    }
}
