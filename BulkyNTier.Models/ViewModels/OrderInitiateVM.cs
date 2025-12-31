using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models.ViewModels
{
    public class OrderInitiateVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public IEnumerable<ShippingAddress> ShippingAddresses { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public ShippingAddress NewShippingAddress { get; set; }
    }

}
