using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models.ViewModels
{
    public class ApplicationUserVM
    {
        public ApplicationUser applicationUser {  get; set; }
        [ValidateNever]
        public Company company { get; set; }
    }

}
