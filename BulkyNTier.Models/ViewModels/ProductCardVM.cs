using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.Models.ViewModels
{
    public class ProductCardVM
    {
        public int Id { get; set; }
        public string ImageURL {  get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public double Price { get; set; }

        public double ListPrice { get; set; }
        public int DiscoutPercentage { get; set; }

    }
}
