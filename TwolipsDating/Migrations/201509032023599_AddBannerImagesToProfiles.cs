namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBannerImagesToProfiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "BannerImageId", c => c.Int());
            AddColumn("dbo.UserImages", "IsBanner", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Profiles", "BannerImageId");
            AddForeignKey("dbo.Profiles", "BannerImageId", "dbo.UserImages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "BannerImageId", "dbo.UserImages");
            DropIndex("dbo.Profiles", new[] { "BannerImageId" });
            DropColumn("dbo.UserImages", "IsBanner");
            DropColumn("dbo.Profiles", "BannerImageId");
        }
    }
}
