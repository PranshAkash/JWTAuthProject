using JWTAuthProject.AppCode.DAL;
using JWTAuthProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;

namespace JWTAuthProject.AppCode.Data
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly IDapperRepository _dapperRepository;
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger, IDapperRepository dapperRepository) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<ApplicationUser> FindByMobileNoAsync(string mobileNo)
        {
            string sqlQuery = @"SELECT u.Id,u.UserName,u.RefreshToken, ar.[Name] as [Role], u.Email,u.PhoneNumber
                                FROM Users u(nolock) 
                                LEFT join UserRoles r(nolock) on r.UserId = u.Id 
                                LEFT join ApplicationRole ar(nolock) on ar.Id = r.RoleId  
                                WHERE u.Email = @mobileNo";
            try
            {
                var user = await _dapperRepository.GetAsync<ApplicationUser>(sqlQuery, new { mobileNo }, commandType: CommandType.Text);
                return user ?? new ApplicationUser();
            }
            catch (Exception ex)
            {
                return new ApplicationUser();
            }
        }

        public Task FindByMobileNoAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetUserByToken(LoginAPIRequest request)
        {
            var user = await _dapperRepository.GetAsync<ApplicationUser>(@"Select u.Id,u.UserName,u.RefreshToken,u.[Name], ar.[Name] as [Role], u.Email from Users u left join UserRoles r on r.UserId = u.Id left join ApplicationRole ar on ar.Id = r.RoleId where u.UserId = @MerchantID and u.ConcurrencyStamp = @SecurityCode",
                new
                {
                    request.MerchantID,
                    request.SecurityCode
                }, commandType: CommandType.Text);
            return user ?? new ApplicationUser();
        }
        public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var result = IdentityResult.Failed();
            try
            {
                var res = await base.ResetPasswordAsync(user, token, newPassword);
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    string sqlQuery = @"update [dbo].[Users] set PasswordHash=@PasswordHash where Id=@Id";
                    int i = await _dapperRepository.ExecuteAsync(sqlQuery, new { user.Id, user.PasswordHash }, commandType: CommandType.Text);
                    if (i > 0)
                    {
                        result = IdentityResult.Success;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
