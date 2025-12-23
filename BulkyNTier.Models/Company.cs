
using System.ComponentModel.DataAnnotations;


namespace BulkyNTier.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? StreetAddress { get; set; } 

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public string? PhoneNumber { get; set; }

    }
}
