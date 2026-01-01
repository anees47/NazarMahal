using Microsoft.AspNetCore.Identity;
using NazarMahal.Core.Common;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public override string? PhoneNumber { get; set; }
        
        public string UserType { get; set; } = RoleConstants.Customer;
        public DateTime DateCreated { get; set; } = PakistanTimeHelper.Now;
        public bool IsDisabled { get; internal set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
