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
       public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
       public double Total { get; set; }
      
    }
}
