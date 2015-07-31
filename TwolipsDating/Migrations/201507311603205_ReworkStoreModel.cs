namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReworkStoreModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserTitles", "TitleId", "dbo.Titles");
            DropForeignKey("dbo.Profiles", "SelectedTitleId", "dbo.Titles");
            DropForeignKey("dbo.GiftTransactionLogs", "GiftId", "dbo.Gifts");
            DropForeignKey("dbo.StoreTransactionLogs", "GiftId", "dbo.Gifts");
            DropForeignKey("dbo.InventoryItems", "GiftId", "dbo.Gifts");
            DropIndex("dbo.Profiles", new[] { "SelectedTitleId" });
            DropIndex("dbo.UserTitles", new[] { "TitleId" });
            DropIndex("dbo.InventoryItems", "UX_OwnerAndGift");
            DropIndex("dbo.GiftTransactionLogs", new[] { "GiftId" });
            DropIndex("dbo.StoreTransactionLogs", new[] { "GiftId" });
            DropPrimaryKey("dbo.UserTitles");
            CreateTable(
                "dbo.StoreItems",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(nullable: false, maxLength: 255),
                        ItemTypeId = c.Int(nullable: false),
                        PointPrice = c.Int(nullable: false),
                        IconFileName = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoreItemTypes", t => t.ItemTypeId)
                .Index(t => t.ItemTypeId);
            
            CreateTable(
                "dbo.StoreItemTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserTitles", "StoreItemId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryItems", "StoreItemId", c => c.Int(nullable: false));
            AddColumn("dbo.GiftTransactionLogs", "StoreItemId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreTransactionLogs", "StoreItemId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserTitles", new[] { "UserId", "StoreItemId" });
            CreateIndex("dbo.Profiles", "SelectedTitleId");
            CreateIndex("dbo.GiftTransactionLogs", "StoreItemId");
            CreateIndex("dbo.InventoryItems", new[] { "ApplicationUserId", "StoreItemId" }, unique: true, name: "UX_OwnerAndGift");
            CreateIndex("dbo.UserTitles", "StoreItemId");
            CreateIndex("dbo.StoreTransactionLogs", "StoreItemId");
            AddForeignKey("dbo.GiftTransactionLogs", "StoreItemId", "dbo.StoreItems", "Id");
            AddForeignKey("dbo.InventoryItems", "StoreItemId", "dbo.StoreItems", "Id");
            AddForeignKey("dbo.UserTitles", "StoreItemId", "dbo.StoreItems", "Id");
            AddForeignKey("dbo.StoreTransactionLogs", "StoreItemId", "dbo.StoreItems", "Id");
            AddForeignKey("dbo.Profiles", "SelectedTitleId", "dbo.StoreItems", "Id");
            DropColumn("dbo.UserTitles", "TitleId");
            DropColumn("dbo.InventoryItems", "GiftId");
            DropColumn("dbo.GiftTransactionLogs", "GiftId");
            DropColumn("dbo.StoreTransactionLogs", "GiftId");
            DropTable("dbo.Titles");
            DropTable("dbo.Gifts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Gifts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(nullable: false, maxLength: 255),
                        IconFileName = c.String(nullable: false, maxLength: 64),
                        PointPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(nullable: false, maxLength: 255),
                        PointPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.StoreTransactionLogs", "GiftId", c => c.Int(nullable: false));
            AddColumn("dbo.GiftTransactionLogs", "GiftId", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryItems", "GiftId", c => c.Int(nullable: false));
            AddColumn("dbo.UserTitles", "TitleId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Profiles", "SelectedTitleId", "dbo.StoreItems");
            DropForeignKey("dbo.StoreTransactionLogs", "StoreItemId", "dbo.StoreItems");
            DropForeignKey("dbo.UserTitles", "StoreItemId", "dbo.StoreItems");
            DropForeignKey("dbo.StoreItems", "ItemTypeId", "dbo.StoreItemTypes");
            DropForeignKey("dbo.InventoryItems", "StoreItemId", "dbo.StoreItems");
            DropForeignKey("dbo.GiftTransactionLogs", "StoreItemId", "dbo.StoreItems");
            DropIndex("dbo.StoreTransactionLogs", new[] { "StoreItemId" });
            DropIndex("dbo.UserTitles", new[] { "StoreItemId" });
            DropIndex("dbo.InventoryItems", "UX_OwnerAndGift");
            DropIndex("dbo.GiftTransactionLogs", new[] { "StoreItemId" });
            DropIndex("dbo.StoreItems", new[] { "ItemTypeId" });
            DropIndex("dbo.Profiles", new[] { "SelectedTitleId" });
            DropPrimaryKey("dbo.UserTitles");
            DropColumn("dbo.StoreTransactionLogs", "StoreItemId");
            DropColumn("dbo.GiftTransactionLogs", "StoreItemId");
            DropColumn("dbo.InventoryItems", "StoreItemId");
            DropColumn("dbo.UserTitles", "StoreItemId");
            DropTable("dbo.StoreItemTypes");
            DropTable("dbo.StoreItems");
            AddPrimaryKey("dbo.UserTitles", new[] { "UserId", "TitleId" });
            CreateIndex("dbo.StoreTransactionLogs", "GiftId");
            CreateIndex("dbo.GiftTransactionLogs", "GiftId");
            CreateIndex("dbo.InventoryItems", new[] { "ApplicationUserId", "GiftId" }, unique: true, name: "UX_OwnerAndGift");
            CreateIndex("dbo.UserTitles", "TitleId");
            CreateIndex("dbo.Profiles", "SelectedTitleId");
            AddForeignKey("dbo.InventoryItems", "GiftId", "dbo.Gifts", "Id");
            AddForeignKey("dbo.StoreTransactionLogs", "GiftId", "dbo.Gifts", "Id");
            AddForeignKey("dbo.GiftTransactionLogs", "GiftId", "dbo.Gifts", "Id");
            AddForeignKey("dbo.Profiles", "SelectedTitleId", "dbo.Titles", "Id");
            AddForeignKey("dbo.UserTitles", "TitleId", "dbo.Titles", "Id");
        }
    }
}
