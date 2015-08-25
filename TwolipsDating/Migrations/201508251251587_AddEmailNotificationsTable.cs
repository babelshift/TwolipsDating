namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailNotificationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailNotifications",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        SendNewFollowerNotifications = c.Boolean(nullable: false),
                        SendTagNotifications = c.Boolean(nullable: false),
                        SendGiftNotifications = c.Boolean(nullable: false),
                        SendMessageNotifications = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ApplicationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailNotifications", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.EmailNotifications", new[] { "ApplicationUserId" });
            DropTable("dbo.EmailNotifications");
        }
    }
}
