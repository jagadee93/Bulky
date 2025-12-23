using BulkyNTier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace BulkyNTier.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {


        public readonly UserManager<ApplicationUser> _userManager;



        [BindProperty]
        public ApplicationUser InputUser { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public AddressModel(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }



        public async Task<IActionResult> OnPostAsync()
        {
            var user=await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Console.WriteLine(user+"User...");

            if (!ModelState.IsValid)
            {
                return Page();
            }


            user.Name=InputUser.Name;
            user.StreetAddress=InputUser.StreetAddress;
            user.City=InputUser.City;
            user.State=InputUser.State;
            user.PostalCode=InputUser.PostalCode;


            //update the user In Db
            var result=await _userManager.UpdateAsync(user);


            if (result.Succeeded)
            {
                StatusMessage = "Address has been updated";
                return Page();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();

        }






        public async Task<IActionResult> OnGetAsync()
        {
            var user=await _userManager.GetUserAsync(User);
            Console.WriteLine(user);
            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            //Load the user into existing Object.

            InputUser = new ApplicationUser
            {
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
            };
            return Page();
        }
    }
}
