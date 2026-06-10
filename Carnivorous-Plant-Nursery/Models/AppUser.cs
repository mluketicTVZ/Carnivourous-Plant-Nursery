using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Carnivorous_Plant_Nursery.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DefaultShippingCity { get; set; }
    }
}
