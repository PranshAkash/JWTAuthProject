using FluentMigrator;
using System.Collections.Generic;

namespace JWTAuthProject.AppCode.Migrations
{
    [Migration(202106280002)]
    public class InitialSeed_202106280002 : Migration
    {
        public override void Down()
        {
            //Delete.FromTable("Employees")
            //    .Row(new
            //    {
            //        EmployeeId = new Guid("59c0d403-71ce-4ac8-9c2c-b0e54e7c043b"),
            //        Age = 34,
            //        Name = "Test Employee",
            //        Position = "Test Position",
            //        CompanyId = new Guid("67fbac34-1ee1-4697-b916-1748861dd275")
            //    });
            //Delete.FromTable("Companies")
            //    .Row(new
            //    {
            //        CompanyId = new Guid("67fbac34-1ee1-4697-b916-1748861dd275"),
            //        Address = "Test Address",
            //        Country = "USA",
            //        Name = "Test Name"
            //    });
        }
        public override void Up()
        {
            Insert.IntoTable("ApplicationRole")
                .Row(new
                {
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }).Row(new
                {
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "USER"
                });
            Insert.IntoTable("Users")
              .Row(new
              {
                  UserId = Guid.NewGuid().ToString(),
                  AccessFailedCount = 0,
                  ConcurrencyStamp = "411ecb83-6ae9-4c86-9364-e85fc290e9ea",
                  Email = "admin@gmail.com",
                  EmailConfirmed = false,
                  LockoutEnabled = false,
                  LockoutEnd = DateTime.Now,
                  NormalizedEmail = "ADMIN@GMAIL.COM",
                  NormalizedUserName = "ADMIN@GMAIL.COM",
                  PasswordHash = "AQAAAAIAAYagAAAAEN8GKnu/hjId0SfDQV/opEXY+aiS4tHzRY/kgbl8qOegY2XNwfiiuKc8Kbspk25tuw==", // Welcome@1
                  PhoneNumberConfirmed = false,
                  PhoneNumber = "0000000000",
                  SecurityStamp = "",
                  TwoFactorEnabled = false,
                  UserName = "admin@gmail.com",
                  IsActive = true
              });
            Insert.IntoTable("UserRoles")
              .Row(new
              {
                  UserId = 1,
                  RoleId = 1
              });
        }
    }
}
