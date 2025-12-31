
using System.ComponentModel.DataAnnotations;

namespace BulkyNTier.Models.ViewModels
{
    public class PriceDetailSummaryVM
    {
        [Display(Name ="Total Amount")]
        public decimal TotalPrice { get; set; }
        public decimal ShippingCharges { get; set; }
        public decimal ListPriceTotal { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal DiscountTotal { get; set; }

        public decimal TotalSavings { get; set; }
    }
}
