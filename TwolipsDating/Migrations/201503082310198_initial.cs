namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        CountryId = c.Int(nullable: false),
                        USStateId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.USStates", t => t.USStateId)
                .Index(t => t.CountryId)
                .Index(t => t.USStateId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Birthday = c.DateTime(nullable: false),
                        GenderId = c.Int(nullable: false),
                        ZipCode = c.Int(),
                        CityId = c.Int(nullable: false),
                        UserImageId = c.Int(),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Genders", t => t.GenderId)
                .Index(t => t.GenderId)
                .Index(t => t.CityId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false, maxLength: 256),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateLastLogin = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderApplicationUserId = c.String(nullable: false, maxLength: 128),
                        ReceiverApplicationUserId = c.String(nullable: false, maxLength: 128),
                        DateSent = c.DateTime(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 255),
                        Body = c.String(nullable: false, maxLength: 2000),
                        MessageStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MessageStatus", t => t.MessageStatusId)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverApplicationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderApplicationUserId)
                .Index(t => t.SenderApplicationUserId)
                .Index(t => t.ReceiverApplicationUserId)
                .Index(t => t.MessageStatusId);
            
            CreateTable(
                "dbo.MessageStatus",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorUserId = c.String(nullable: false, maxLength: 128),
                        TargetUserId = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        Content = c.String(nullable: false, maxLength: 2000),
                        RatingValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorUserId)
                .ForeignKey("dbo.ReviewRatings", t => t.RatingValue)
                .ForeignKey("dbo.AspNetUsers", t => t.TargetUserId)
                .Index(t => new { t.AuthorUserId, t.TargetUserId }, unique: true, name: "UX_AuthorAndTarget")
                .Index(t => t.RatingValue);
            
            CreateTable(
                "dbo.ReviewRatings",
                c => new
                    {
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Value);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        FileName = c.String(nullable: false, maxLength: 64),
                        DateUploaded = c.DateTime(nullable: false),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.Genders",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
            CreateTable(
                "dbo.USStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Abbreviation = c.String(nullable: false, maxLength: 2),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Abbreviation, unique: true, name: "UX_Abbreviation")
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
            CreateTable(
                "dbo.ZipCodes",
                c => new
                    {
                        ZipCodeId = c.String(nullable: false, maxLength: 5),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ZipCodeId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ZipCodes", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Cities", "USStateId", "dbo.USStates");
            DropForeignKey("dbo.UserImages", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.Profiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Profiles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserImages", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "TargetUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "RatingValue", "dbo.ReviewRatings");
            DropForeignKey("dbo.Reviews", "AuthorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "SenderApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "ReceiverApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "MessageStatusId", "dbo.MessageStatus");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ZipCodes", new[] { "CityId" });
            DropIndex("dbo.USStates", "UX_Name");
            DropIndex("dbo.USStates", "UX_Abbreviation");
            DropIndex("dbo.Genders", "UX_Name");
            DropIndex("dbo.UserImages", new[] { "Profile_Id" });
            DropIndex("dbo.UserImages", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "RatingValue" });
            DropIndex("dbo.Reviews", "UX_AuthorAndTarget");
            DropIndex("dbo.MessageStatus", "UX_Name");
            DropIndex("dbo.Messages", new[] { "MessageStatusId" });
            DropIndex("dbo.Messages", new[] { "ReceiverApplicationUserId" });
            DropIndex("dbo.Messages", new[] { "SenderApplicationUserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Profiles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Profiles", new[] { "CityId" });
            DropIndex("dbo.Profiles", new[] { "GenderId" });
            DropIndex("dbo.Countries", "UX_Name");
            DropIndex("dbo.Cities", new[] { "USStateId" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ZipCodes");
            DropTable("dbo.USStates");
            DropTable("dbo.Genders");
            DropTable("dbo.UserImages");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.ReviewRatings");
            DropTable("dbo.Reviews");
            DropTable("dbo.MessageStatus");
            DropTable("dbo.Messages");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Profiles");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
        }
    }
}
