using System.ComponentModel.DataAnnotations;

namespace Carnivorous_Plant_Nursery.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; } = string.Empty;

        [Phone]
        [RegularExpression(@"^\+?[0-9\s\-()]{6,20}$", ErrorMessage = ErrorMessage.InvalidPhoneNumberFormat)]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(200)]
        [Display(Name = "Default shipping city")]
        public string? DefaultShippingCity { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = ErrorMessage.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = ErrorMessage.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = ErrorMessage.PasswordConfirmationMismatch)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AccountManageViewModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; } = string.Empty;

        [Phone]
        [RegularExpression(@"^\+?[0-9\s\-()]{6,20}$", ErrorMessage = ErrorMessage.InvalidPhoneNumberFormat)]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(200)]
        [RegularExpression(@"^[\p{L}\s'.-]*$", ErrorMessage = ErrorMessage.InvalidCityFormat)]
        [Display(Name = "Default shipping city")]
        public string? DefaultShippingCity { get; set; }

        public bool HasLocalPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string? CurrentPassword { get; set; }

        [MinLength(8, ErrorMessage = ErrorMessage.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [MinLength(8, ErrorMessage = ErrorMessage.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = ErrorMessage.PasswordConfirmationMismatch)]
        [Display(Name = "Confirm new password")]
        public string? ConfirmNewPassword { get; set; }
    }
}
