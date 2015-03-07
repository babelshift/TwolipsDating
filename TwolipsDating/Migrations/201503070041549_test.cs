namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        FileName = c.String(nullable: false, maxLength: 64),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.Profile_Id);
            
            AddColumn("dbo.Profiles", "UserImageId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserImages", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.UserImages", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserImages", new[] { "Profile_Id" });
            DropIndex("dbo.UserImages", new[] { "ApplicationUserId" });
            DropColumn("dbo.Profiles", "UserImageId");
            DropTable("dbo.UserImages");
        }
    }
}
