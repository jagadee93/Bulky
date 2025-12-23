using BulkyNTier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace BulkyNTier.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {


        public readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        [BindProperty]
        public ApplicationUser InputUser { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public AddressModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }



        //public  async void TaskLoadUser()
        //{
        //    await _userManager
        //}


  

        public async Task<IActionResult> OnGetAsync()
        {
            var user=await _userManager.GetUserAsync(User);
            Console.WriteLine(user);
            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return Page();
        }
    }
}
