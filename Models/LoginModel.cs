using System.ComponentModel.DataAnnotations;

namespace TenderTracker.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [DataType(DataType.EmailAddress)]
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$", ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^(?=.{1,50}$)(?!.*\s)[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.com$", ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; } = "null";


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters long.")]
        //[DataType(DataType.Password)]
        //[Required(ErrorMessage = "Password must be atleast 5 characters")]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{5,}$", ErrorMessage = "Please enter a valid password.")]
        public string? Password { get; set; } = "null";

        [Required(ErrorMessage = "CAPTCHA is required.")]
        public string? CaptchaCode { get; set; } = "null";
    }
}
