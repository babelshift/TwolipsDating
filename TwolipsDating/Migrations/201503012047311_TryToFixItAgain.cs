namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TryToFixItAgain : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Profiles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Profiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Profiles", "GenderId", "dbo.Genders");
            DropIndex("dbo.Profiles", new[] { "GenderId" });
            DropIndex("dbo.Profiles", new[] { "CityId" });
            DropIndex("dbo.Profiles", new[] { "ApplicationUser_Id" });
            AddColumn("dbo.AspNetUsers", "ProfileId", c => c.Int());
            DropTable("dbo.Profiles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Age = c.Int(nullable: false),
                        GenderId = c.Int(nullable: false),
                        ZipCode = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        ApplicationUserId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.AspNetUsers", "ProfileId");
            CreateIndex("dbo.Profiles", "ApplicationUser_Id");
            CreateIndex("dbo.Profiles", "CityId");
            CreateIndex("dbo.Profiles", "GenderId");
            AddForeignKey("dbo.Profiles", "GenderId", "dbo.Genders", "Id");
            AddForeignKey("dbo.Profiles", "CityId", "dbo.Cities", "Id");
            AddForeignKey("dbo.Profiles", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
