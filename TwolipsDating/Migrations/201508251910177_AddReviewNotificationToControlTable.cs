namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewNotificationToControlTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailNotifications", "SendReviewNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailNotifications", "SendReviewNotifications");
        }
    }
}
