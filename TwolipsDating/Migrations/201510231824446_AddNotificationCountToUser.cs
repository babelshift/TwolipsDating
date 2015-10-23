namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotificationCountToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "NotificationCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "NotificationCount");
        }
    }
}
