using JWTAuthProject.AppCode.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthProject.Controllers
{
    [JWTAuthorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
