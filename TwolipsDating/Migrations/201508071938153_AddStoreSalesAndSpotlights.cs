namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreSalesAndSpotlights : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreSales",
                c => new
                    {
                        SaleId = c.Int(nullable: false, identity: true),
                        StoreItemId = c.Int(nullable: false),
                        Discount = c.Double(nullable: false),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SaleId)
                .ForeignKey("dbo.StoreItems", t => t.StoreItemId)
                .Index(t => new { t.StoreItemId, t.DateStart, t.DateEnd }, unique: true, name: "UX_StoreItemAndDate");
            
            CreateTable(
                "dbo.StoreGiftSpotlights",
                c => new
                    {
                        StoreSaleId = c.Int(nullable: false),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreSaleId, t.DateStart, t.DateEnd })
                .ForeignKey("dbo.StoreSales", t => t.StoreSaleId)
                .Index(t => t.StoreSaleId);
            
            CreateTable(
                "dbo.StoreSpotlights",
                c => new
                    {
                        StoreSaleId = c.Int(nullable: false),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreSaleId, t.DateStart, t.DateEnd })
                .ForeignKey("dbo.StoreSales", t => t.StoreSaleId)
                .Index(t => t.StoreSaleId);
            
            CreateIndex("dbo.Quizs", "Name", unique: true, name: "UX_Name");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreSpotlights", "StoreSaleId", "dbo.StoreSales");
            DropForeignKey("dbo.StoreSales", "StoreItemId", "dbo.StoreItems");
            DropForeignKey("dbo.StoreGiftSpotlights", "StoreSaleId", "dbo.StoreSales");
            DropIndex("dbo.StoreSpotlights", new[] { "StoreSaleId" });
            DropIndex("dbo.StoreGiftSpotlights", new[] { "StoreSaleId" });
            DropIndex("dbo.StoreSales", "UX_StoreItemAndDate");
            DropIndex("dbo.Quizs", "UX_Name");
            DropTable("dbo.StoreSpotlights");
            DropTable("dbo.StoreGiftSpotlights");
            DropTable("dbo.StoreSales");
        }
    }
}
