namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gifts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventoryItems",
                c => new
                    {
                        InventoryItemId = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        GiftId = c.Int(nullable: false),
                        ItemCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryItemId)
                .ForeignKey("dbo.Gifts", t => t.GiftId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => new { t.ApplicationUserId, t.GiftId }, unique: true, name: "UX_OwnerAndGift");
            
            CreateTable(
                "dbo.Gifts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(nullable: false, maxLength: 255),
                        IconFileName = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GiftTransactionLogs",
                c => new
                    {
                        GiftTransactionLogId = c.Int(nullable: false, identity: true),
                        FromUserId = c.String(nullable: false, maxLength: 128),
                        ToUserId = c.String(nullable: false, maxLength: 128),
                        GiftId = c.Int(nullable: false),
                        ItemCount = c.Int(nullable: false),
                        DateTransactionOccurred = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GiftTransactionLogId)
                .ForeignKey("dbo.AspNetUsers", t => t.FromUserId)
                .ForeignKey("dbo.Gifts", t => t.GiftId)
                .ForeignKey("dbo.AspNetUsers", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId)
                .Index(t => t.GiftId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryItems", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InventoryItems", "GiftId", "dbo.Gifts");
            DropForeignKey("dbo.GiftTransactionLogs", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiftTransactionLogs", "GiftId", "dbo.Gifts");
            DropForeignKey("dbo.GiftTransactionLogs", "FromUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GiftTransactionLogs", new[] { "GiftId" });
            DropIndex("dbo.GiftTransactionLogs", new[] { "ToUserId" });
            DropIndex("dbo.GiftTransactionLogs", new[] { "FromUserId" });
            DropIndex("dbo.InventoryItems", "UX_OwnerAndGift");
            DropTable("dbo.GiftTransactionLogs");
            DropTable("dbo.Gifts");
            DropTable("dbo.InventoryItems");
        }
    }
}
