using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BulkyNTier.Models
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        
    }

    public enum PaymentStatus
    {
        Pending,
        Approved,
        Rejected,
        Refunded,
        ApprovedForDelayedPayment,
        Cancelled,
    }

    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public int ShippingAddressId { get; set; }
        [ForeignKey(nameof(ShippingAddressId))]
        [ValidateNever]
        public ShippingAddress ShippingAddress { get; set; }


        public DateTime OrderDate { get; set; }
    
        public DateTime? ExpectedShippingDate {  get; set; }

        public decimal OrderTotal { get; set; }

        public string? TrackingNumber { get; set; }
        public string? Carrier {  get; set; }
        
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? PaymentDate { get; set; }
        public DateOnly? PaymentDueDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }



        public ICollection<OrderDetail> OrderDetails { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
