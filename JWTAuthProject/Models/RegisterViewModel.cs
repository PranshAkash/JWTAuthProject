using JWTAuthProject.AppCode.Enums;
using FluentMigrator.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace JWTAuthProject.Models
{
    public class Register : Response
    {
        public int Id { get; set; }
        public Role Role { get; set; } = Role.User;

        [Required(ErrorMessage = "Please enter name")]
        [StringLength(200)]
        [RegularExpression(@"[a-zA-Z ]*$", ErrorMessage = "Only Alphabets are Allowed")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Mobile")]
        [StringLength(10)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(0|91)?[6-9][0-9]{9}", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Invalid Email Address")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
    }
    public class RegisterViewModel : Register
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password did not match..")]
        public string ConfirmPassword { get; set; }
    }
}
