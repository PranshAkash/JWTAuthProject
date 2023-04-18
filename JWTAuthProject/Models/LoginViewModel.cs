using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace JWTAuthProject.Models
{
    public class LoginViewModel
    {
        [Required]
        public string MobileNo { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        [NotMapped]
        public bool IsTwoFactorEnabled { get; set; }
    }
}
