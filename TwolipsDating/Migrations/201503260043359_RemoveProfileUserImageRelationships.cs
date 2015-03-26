namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProfileUserImageRelationships : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserImages", "Profile_Id", "dbo.Profiles");
            DropIndex("dbo.UserImages", new[] { "Profile_Id" });
            DropColumn("dbo.Profiles", "UserImageId");
            DropColumn("dbo.UserImages", "Profile_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserImages", "Profile_Id", c => c.Int());
            AddColumn("dbo.Profiles", "UserImageId", c => c.Int());
            CreateIndex("dbo.UserImages", "Profile_Id");
            AddForeignKey("dbo.UserImages", "Profile_Id", "dbo.Profiles", "Id");
        }
    }
}
