namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        NotificationTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.NotificationTypeId);
            
            CreateTable(
                "dbo.NotificationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "NotificationTypeId", "dbo.NotificationTypes");
            DropForeignKey("dbo.Notifications", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "NotificationTypeId" });
            DropIndex("dbo.Notifications", new[] { "ApplicationUserId" });
            DropTable("dbo.NotificationTypes");
            DropTable("dbo.Notifications");
        }
    }
}
