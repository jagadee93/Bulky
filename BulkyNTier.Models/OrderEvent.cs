using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BulkyNTier.Models
{
    public enum OrderEventType
    {
        Created,
        PaymentInitiated,
        PaymentCompleted,
        PaymentFailed,
        Approved,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }

    public class OrderEvent
    {
        public int Id { get; set; }

        public int OrderHeaderId { get; set; }

        [ForeignKey(nameof(OrderHeaderId))]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        public OrderEventType EventType { get; set; }

        public string? Description { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        public string? PerformedByUserId { get; set; }
    }

}


