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
    public enum AddressType{
        Home,
        Work,
        Other,
        Friends,
    }
    public class ShippingAddress
    {

        public int Id { get; set; }
        [Required,MaxLength(30)]
        public string Name {  get; set; }
        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Phone No must be 10 digits"), Display(Name="Phone")]
        public string PhoneNumber { get; set; }

        [Display(Name ="Alternate Phone"),RegularExpression(@"^\d{10}$", ErrorMessage = "Alternate Phone no must be 10 digits")]
        public string? AlternatePhoneNumber {  get; set; }

        [Required]
        public AddressType AddressType { get; set; }

        [Required,MaxLength(30)]
        public string StreetAddress { get; set; }

        [Required,MaxLength(20)]
        public string City { get; set; }

        [Required,MaxLength(25)]
        
        public string State {  get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Postal code must be 6 digits")]
        public string PostalCode { get; set; }

        [MaxLength(20)]
        public string? Country { get; set; }

        public bool IsDefaultAddress { get; set; }

        [ValidateNever]
        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]

        public ApplicationUser ApplicationUser { get; set; }




        
    }
}
