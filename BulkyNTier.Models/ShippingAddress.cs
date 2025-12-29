using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models
{
    public class ShippingAddress
    {

        public int Id { get; set; }
        [Required,MaxLength(30)]
        public string Name {  get; set; }
        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Phone No must be 10 digits"), Display(Name="Phone")]
        public string PhoneNumber { get; set; }

        [Display(Name ="Alternate Phone"),RegularExpression(@"^\d{10}$", ErrorMessage = "Alternate Phone no must be 10 digits")]
        public string AlternatePhoneNumber {  get; set; }


        public string AddressType { get; set; }

        [Required,MaxLength(16)]
        public string StreetAddress { get; set; }

        [Required,MaxLength(15)]
        public string City { get; set; }

        [Required,MaxLength(20)]
        
        public string State {  get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Postal code must be 6 digits")]
        public string PostalCode { get; set; }

        [MaxLength(16)]
        public string Country { get; set; }

        public bool IsDefaultAddress { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]

        public ApplicationUser ApplicationUser { get; set; }



        
    }
}
