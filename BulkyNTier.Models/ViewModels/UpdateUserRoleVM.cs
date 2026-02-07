using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models.ViewModels
{
    public class UpdateUserRoleVM
    {
        public string UserID { get; set; }
        public string UserName {  get; set; }
        public string Role {  get; set; }


        [Display(Name ="Company")]
        public int? CompanyId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CompaniesList { get; set; } = new List<SelectListItem>();
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get;set; } = new List<SelectListItem>();
    }
}
