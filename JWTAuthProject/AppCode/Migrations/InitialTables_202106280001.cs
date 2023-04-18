using FluentMigrator;

namespace JWTAuthProject.AppCode.Migrations
{
    [Migration(202106280001)]
    public class InitialTables_202106280001 : Migration
    {
        public override void Down()
        {
            //Delete.Table("Employees");
            //Delete.Table("Companies");
        }

        public override void Up()
        {
            /* Role */
            Create.Table("ApplicationRole")
               .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
               .WithColumn("ConcurrencyStamp").AsString(1000).NotNullable()
               .WithColumn("Name").AsString(256).NotNullable()
               .WithColumn("NormalizedName").AsString(256).NotNullable();
            /* Role Claim */
            Create.Table("RoleClaims")
               .WithColumn("Id").AsInt64().NotNullable().Identity()
               .WithColumn("ClaimType").AsString(1000).NotNullable()
               .WithColumn("ClaimValue").AsString(1000).NotNullable()
               .WithColumn("RoleId").AsInt64().NotNullable()
               .WithColumn("Country").AsString(50).NotNullable();

            /* Users */
            Create.Table("Users")
               .WithColumn("Id").AsInt64().NotNullable().Identity()
               .WithColumn("UserId").AsString(1000).NotNullable().PrimaryKey()
               .WithColumn("AccessFailedCount").AsInt64()
               .WithColumn("ConcurrencyStamp").AsString(1000)
               .WithColumn("Email").AsString(256).NotNullable()
               .WithColumn("EmailConfirmed").AsBoolean().NotNullable()
               .WithColumn("LockoutEnabled").AsBoolean()
               .WithColumn("LockoutEnd").AsDateTime()
               .WithColumn("NormalizedEmail").AsString(256).NotNullable()
               .WithColumn("NormalizedUserName").AsString(256).NotNullable()
               .WithColumn("PasswordHash").AsString(256).NotNullable()
               .WithColumn("PhoneNumberConfirmed").AsBoolean().NotNullable()
               .WithColumn("PhoneNumber").AsString()
               .WithColumn("SecurityStamp").AsString(1000)
               .WithColumn("TwoFactorEnabled").AsBoolean().NotNullable().WithDefaultValue(false)
               .WithColumn("UserName").AsString(256).NotNullable()
               .WithColumn("RefreshToken").AsString(256).Nullable()
               .WithColumn("RefreshTokenExpiryTime").AsDateTime().Nullable()
               .WithColumn("IsActive").AsBoolean().Nullable().WithDefaultValue(true);

            /* UserClaims */
            Create.Table("UserClaims")
               .WithColumn("Id").AsInt64().Identity().NotNullable()
               .WithColumn("ClaimType").AsString(1000)
               .WithColumn("ClaimValue").AsString(1000)
               .WithColumn("UserId").AsString(256).NotNullable();

            /* UserLogins */
            Create.Table("UserLogins")
               .WithColumn("Id").AsInt64().Identity().NotNullable()
               .WithColumn("LoginProvider").AsString(128)
               .WithColumn("ProviderKey").AsString(128)
               .WithColumn("ProviderDisplayName").AsString(1000)
               .WithColumn("UserId").AsString(256).NotNullable();

            /* UserTokens */
            Create.Table("UserTokens")
               .WithColumn("Id").AsInt64().Identity().NotNullable()
               .WithColumn("UserId").AsString(256).NotNullable()
               .WithColumn("LoginProvider").AsString(128).NotNullable()
               .WithColumn("Name").AsString(128).NotNullable()
               .WithColumn("Value").AsString(128).NotNullable();

            /* UserRoles */
            Create.Table("UserRoles")
               .WithColumn("Id").AsInt64().Identity().NotNullable()
               .WithColumn("UserId").AsString(256).NotNullable()
               .WithColumn("RoleId").AsString(128).NotNullable();

            /* NLogs */
            Create.Table("NLogs")
               .WithColumn("Id").AsInt64().Identity().NotNullable()
               .WithColumn("When").AsString()
               .WithColumn("Message").AsString()
               .WithColumn("Level").AsString()
               .WithColumn("Exception").AsString()
               .WithColumn("Trace").AsString()
               .WithColumn("Logger").AsString();

            /* ErrorLog*/
            Create.Table("ErrorLog")
               .WithColumn("Id").AsInt64().Identity().NotNullable()
               .WithColumn("ErrorMsg").AsString(240).NotNullable()
               .WithColumn("ErrorFrom").AsString(50).NotNullable()
               .WithColumn("ErrorNumber").AsString(10).NotNullable()
               .WithColumn("EntryOn").AsDateTime().NotNullable();
        }
    }
}
