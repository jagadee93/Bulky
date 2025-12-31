using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models.ViewModels
{
    public class ShippingAddressVM
    {
       public IEnumerable<ShippingAddress> ShippingAddresses {  get; set; }
       public ShippingAddress NewShippingAddress {  get; set; }

    }
}
