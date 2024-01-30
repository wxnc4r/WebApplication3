using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;

namespace WebApplication3.ViewModels
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null) {
                var email = value.ToString();
                var userManager = (UserManager<UserApplication>)validationContext.GetService(typeof(UserManager<UserApplication>));

                var existingUser = userManager.FindByEmailAsync(email).Result;

                if (existingUser != null)
                {
                    return new ValidationResult("Email already exists.");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class Register
    {

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "only alphabets.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "only alphabets.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Required]
        [UniqueEmail]
        [RegularExpression(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()-_+=])[A-Za-z\d!@#$%^&*()-_+=]{12,}$", ErrorMessage = "min length 12, must include uppercase letters, lowercase letters, digits, and special characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Resume { get; set; }

        [BindNever]
        [DataType(DataType.Text)]
        public string ResumeFilePath { get; set; } = "placeholder";

        [Required]
        [DataType(DataType.Text)]
        public string WhoAmI { get; set; }
    }
}
