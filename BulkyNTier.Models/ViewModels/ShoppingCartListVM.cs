using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models.ViewModels
{
    public class ShoppingCartListVM
    {
        [ValidateNever]
       public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        [ValidateNever]
        public IEnumerable<ShippingAddress> ShippingAddresses { get; set; }

       public ShippingAddress NewShippingAddress { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [ValidateNever]
        public int? SelectedShippingAddressId { get; set; }
      
    }
}
