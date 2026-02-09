
#nullable disable

using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using BulkyNTier.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace BulkyNTier.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private IUnitOfWork _unitOfWork { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork
            )
            
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Role { get; set; }

            public int? CompanyId { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

            [ValidateNever]
            public IEnumerable<SelectListItem> CompaniesList { get; set; } = new List<SelectListItem>();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // ensure roles exist
          

            Input = new InputModel
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }).ToList(),

                 CompaniesList = _unitOfWork.CompanyRepository.GetAll(includeProperties:null).Select(x => new SelectListItem
                 {
                       Text = x.Name,
                       Value = x.Id.ToString()
                 }).ToList()
            };

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            { 
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None); // note: corrected variable below
                                                                                            //Add the company Id
                if (Input.Role == SD.Role_Company)
                {
                  user.CompanyId = Input.CompanyId;
                }
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // assign role
                    if (!string.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }

                    
                  

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (User.IsInRole(SD.Role_Admin))
                        {
                            TempData["success"] = "New User Created successfully";
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            await MergeWishlistAfterLogin(userId);
                        }
                       
                       
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            Input = new InputModel
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                 CompaniesList = _unitOfWork.CompanyRepository.GetAll(includeProperties:null).Select(x => new SelectListItem
                 {
                     Text = x.Name,
                     Value = x.Id.ToString()
                 })


            };
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not abstract and has a parameterless constructor, or override the register page.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }


        private async Task MergeWishlistAfterLogin(string userId)
        {
            var sessionData = HttpContext.Session.GetString(SD.WishListSessionKey);

            if (sessionData == null) return;

            var sessionWishlist = JsonConvert.DeserializeObject<List<int>>(sessionData);
            if (sessionWishlist == null) return;

            foreach (var productId in sessionWishlist)
            {
                bool exists = _unitOfWork.WishListRepository
                    .GetFirstOrDefault(w => w.ApplicationUserId == userId && w.ProductId == productId, includeProperties: null) != null;

                if (!exists)
                {
                    _unitOfWork.WishListRepository.Add(new WishListItem
                    {
                        ApplicationUserId = userId,
                        ProductId = productId
                    });
                }
            }

            _unitOfWork.Save();

            // clear guest wishlist
            HttpContext.Session.Remove(SD.WishListSessionKey);
        }
    }
}
