namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteNotifications : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Notifications", "NotificationTypeId", "dbo.NotificationTypes");
            DropIndex("dbo.Notifications", new[] { "ApplicationUserId" });
            DropIndex("dbo.Notifications", new[] { "NotificationTypeId" });
            DropTable("dbo.Notifications");
            DropTable("dbo.NotificationTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NotificationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        NotificationTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Notifications", "NotificationTypeId");
            CreateIndex("dbo.Notifications", "ApplicationUserId");
            AddForeignKey("dbo.Notifications", "NotificationTypeId", "dbo.NotificationTypes", "Id");
            AddForeignKey("dbo.Notifications", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
