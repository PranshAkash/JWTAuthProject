using JWTAuthProject.AppCode.DAL;
using JWTAuthProject.AppCode.Data;
using JWTAuthProject.AppCode.Interface;
using JWTAuthProject.AppCode.Service;
using JWTAuthProject.Models;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace JWTAuthProject.AppCode.Helper
{
    public static class ServiceCollectionExtension
    {
        public const string corsPolicy = "AllowSpecificOrigin";
        public static void RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            string dbConnectionString = configuration.GetConnectionString("SqlConnection"); // Here SqlConnection is the field in appsetting.json
            IConnectionString ch = new ConnectionString { connectionString = dbConnectionString };
            services.AddSingleton<IConnectionString>(ch);
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<AppCode.Migrations.Database>();
            //  AddFluentMigrator is used for running migration in tables for identity or relation and mapping
            services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer2016()
                .WithGlobalConnectionString(configuration.GetConnectionString("SqlConnection"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

            // JWT config from Configuration file this is how we get data from config files
            JWTConfig jwtConfig = new JWTConfig();
            configuration.GetSection("JWT").Bind(jwtConfig);
            services.AddSingleton(jwtConfig);
            services.Configure<JWTConfig>(configuration.GetSection("JWT"));

            services.AddAutoMapper(typeof(Program));
            #region Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.User.RequireUniqueEmail = false;
            }).AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddUserManager<ApplicationUserManager>()
            .AddDefaultTokenProviders();
            services.AddAuthentication(option =>
            {
                option = new Microsoft.AspNetCore.Authentication.AuthenticationOptions
                {
                    DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
                    DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
                    DefaultScheme = JwtBearerDefaults.AuthenticationScheme
                };
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secretkey"]))
                };
            });
            #endregion


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy,
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }
    }
}
