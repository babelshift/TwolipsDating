namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddProfile : DbMigration
    {
        public override void Up()
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
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Genders", t => t.GenderId)
                .Index(t => t.GenderId)
                .Index(t => t.CityId)
                .Index(t => t.ApplicationUser_Id);
            
            DropColumn("dbo.AspNetUsers", "ProfileId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ProfileId", c => c.Int());
            DropForeignKey("dbo.Profiles", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.Profiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Profiles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Profiles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Profiles", new[] { "CityId" });
            DropIndex("dbo.Profiles", new[] { "GenderId" });
            DropTable("dbo.Profiles");
        }
    }
}
