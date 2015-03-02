namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedZipCodeCity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ZipCodes",
                c => new
                    {
                        ZipCodeId = c.String(nullable: false, maxLength: 5),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ZipCodeId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .Index(t => t.CityId);
            
            DropColumn("dbo.Cities", "ZipCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cities", "ZipCode", c => c.Int());
            DropForeignKey("dbo.ZipCodes", "CityId", "dbo.Cities");
            DropIndex("dbo.ZipCodes", new[] { "CityId" });
            DropTable("dbo.ZipCodes");
        }
    }
}
