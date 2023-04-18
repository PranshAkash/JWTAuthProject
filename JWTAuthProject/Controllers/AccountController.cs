using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;
using JWTAuthProject.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using JWTAuthProject.AppCode.Data;
using JWTAuthProject.AppCode.Interface;
using JWTAuthProject.AppCode.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using JWTAuthProject.AppCode.Helper;

namespace JWTAuthProject.Controllers
{
    public class AccountController : Controller
    {
        #region Variables
        private readonly JWTConfig _jwtConfig;
        private readonly ApplicationUserManager _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IUserService _users;
        private readonly ILogger<AccountController> _logger;
        private readonly ITokenService _tokenService;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        public AccountController(IHttpContextAccessor httpContextAccessor,IOptions<JWTConfig> jwtConfig,
            ApplicationUserManager userManager, RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager, IUserService users, ITokenService tokenService,
            ILogger<AccountController> logger, IMapper mapper
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtConfig = jwtConfig.Value;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _users = users;
            _tokenService = tokenService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            Response response = new Response()
            {
                StatusCode = ResponseStatus.warning,
                ResponseText = "Registration Failed"
            };
            if (ModelState.IsValid)
            {
                var EmailId = model.Email;
                var user = new ApplicationUser
                {

                    UserId = Guid.NewGuid().ToString(),
                    UserName = model.Email.Trim(),
                    Email = model.Email.Trim(),
                    Role = Role.User.ToString(),
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber
                };
                var res = await _userManager.CreateAsync(user, model.Password);
                if (res.Succeeded)
                {
                    user = _userManager.FindByEmailAsync(user.Email).Result;
                    await _userManager.AddToRoleAsync(user, Role.User.ToString());
                    model.Password = string.Empty;
                    model.Email = string.Empty;
                    response.StatusCode = ResponseStatus.Success;
                    response.ResponseText = "User Register Successfully";
                    ModelState.Clear();
                    model = new RegisterViewModel();
                }
                model.ResponseText = response.ResponseText;
                model.StatusCode = response.StatusCode;
                if (res.Errors?.Count() > 0)
                {
                    model.ResponseText = res.Errors.FirstOrDefault().Description;
                }
                return View(model);
            }
            return View(model);
        }


        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View(new LoginViewModel { IsTwoFactorEnabled = false });
        }


        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            OSInfo oSInfo = new OSInfo(_httpContextAccessor);
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid Credentials"
            };
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.MobileNo, model.Password, model.RememberMe, lockoutOnFailure: true);
                 if (result.Succeeded)
                {
                    res = await GenerateAccessToken(model.MobileNo);
                    if (res.Result.Role.ToUpper() == "ADMIN")
                    {
                        return LocalRedirect("/Admin/Index");
                    }
                }
            }
            catch (Exception ex)
            {
                res.ResponseText = "Something went wrong.Please try after some time.";
                _logger.LogError(ex, ex.Message);
            }
            return View(model);
        }


        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> UsersDetails(string role)
        {
            if (role.Trim() != "Fos" && role.Trim() != "Consumer")
            {
                role = string.Empty;
            }
            var users = _users.GetAllAsync().Result;
            if (users.Count() > 0)
            {
                users = users.Where(x => x.Role == role);
            }
            return PartialView("~/Views/Account/PartialView/_UsersList.cshtml", users);
        }

        [HttpPost]
        public async Task<IActionResult> UsersDropdown(int id, string role)
        {
            var users = await _users.GetAllAsync(new ApplicationUser { Id = id, Role = role });
            return Json(users.Select(x => new { x.UserId, x.Email, x.Name }).ToList());
        }

        private async Task<Response<AuthenticateResponse>> GenerateAccessToken(string mobileNo)
        {
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid Credentials"
            };
            var user = await _userManager.FindByMobileNoAsync(mobileNo);

            // Generate Referral Link And Code
            int len = AppConst.StartNumber.Length - user.Id.ToString().Length;
            var Numbers = GenerateZero(len);
            // End Referral

            if (user.Id > 0)
            {
                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypesExtension.Id, user.Id.ToString()),
                        new Claim(ClaimTypesExtension.Role, user.Role ?? "2" ),
                        new Claim(ClaimTypes.Role, user.Role ?? "2" ),
                        new Claim(ClaimTypesExtension.UserName, user.UserName),
                    };
                var token = _tokenService.GenerateAccessToken(claims);
                var authResponse = new AuthenticateResponse(user, token);
                res.StatusCode = ResponseStatus.Success;
                res.ResponseText = "Login Succussful";
                res.Result = authResponse;
            }
            return res;
        }
        private string GenerateZero(int Len)
        {
            StringBuilder res = new StringBuilder();
            for (int i = 1; i <= Len; i++)
            {
                res.Append("0");
            }
            return res.ToString();
        }
    }
}
