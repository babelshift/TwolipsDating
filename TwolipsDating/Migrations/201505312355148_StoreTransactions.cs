namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreTransactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreTransactionLogs",
                c => new
                    {
                        StoreTransactionLogId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        GiftId = c.Int(nullable: false),
                        ItemCount = c.Int(nullable: false),
                        DateTransactionOccurred = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StoreTransactionLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Gifts", t => t.GiftId)
                .Index(t => t.UserId)
                .Index(t => t.GiftId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreTransactionLogs", "GiftId", "dbo.Gifts");
            DropForeignKey("dbo.StoreTransactionLogs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.StoreTransactionLogs", new[] { "GiftId" });
            DropIndex("dbo.StoreTransactionLogs", new[] { "UserId" });
            DropTable("dbo.StoreTransactionLogs");
        }
    }
}
