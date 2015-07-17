namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewBooleanToGiftTransactionLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiftTransactionLogs", "IsReviewedByToUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GiftTransactionLogs", "IsReviewedByToUser");
        }
    }
}
