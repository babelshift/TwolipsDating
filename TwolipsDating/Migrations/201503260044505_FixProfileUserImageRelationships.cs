namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixProfileUserImageRelationships : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "UserImageId", c => c.Int());
            CreateIndex("dbo.Profiles", "UserImageId");
            AddForeignKey("dbo.Profiles", "UserImageId", "dbo.UserImages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "UserImageId", "dbo.UserImages");
            DropIndex("dbo.Profiles", new[] { "UserImageId" });
            DropColumn("dbo.Profiles", "UserImageId");
        }
    }
}
