namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileViewLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProfileViewLogs",
                c => new
                    {
                        ViewerUserId = c.String(nullable: false, maxLength: 128),
                        TargetProfileId = c.Int(nullable: false),
                        DateVisited = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ViewerUserId, t.TargetProfileId })
                .ForeignKey("dbo.Profiles", t => t.TargetProfileId)
                .ForeignKey("dbo.AspNetUsers", t => t.ViewerUserId)
                .Index(t => t.ViewerUserId)
                .Index(t => t.TargetProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfileViewLogs", "ViewerUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProfileViewLogs", "TargetProfileId", "dbo.Profiles");
            DropIndex("dbo.ProfileViewLogs", new[] { "TargetProfileId" });
            DropIndex("dbo.ProfileViewLogs", new[] { "ViewerUserId" });
            DropTable("dbo.ProfileViewLogs");
        }
    }
}
