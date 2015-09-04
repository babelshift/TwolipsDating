namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBannerImagePosition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "BannerPositionX", c => c.Int());
            AddColumn("dbo.Profiles", "BannerPositionY", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "BannerPositionY");
            DropColumn("dbo.Profiles", "BannerPositionX");
        }
    }
}
