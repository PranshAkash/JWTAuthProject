using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace JWTAuthProject.AppCode.Enums
{
    public enum ResponseStatus
    {
        Expired = -2,
        Error = -1,
        Failed = -1,
        Success = 1,
        Pending = 2,
        info = 3,
        warning = 4,
        [Display(Name = "Choose Terminal Id")]
        ChooseTerminalId = 5
    }
    public enum Role
    {
        Admin = 1,
        User = 2,
    }
}
