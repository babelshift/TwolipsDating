namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GeoLocations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GeoCities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GeoStateId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeoStates", t => t.GeoStateId)
                .Index(t => t.GeoStateId);
            
            CreateTable(
                "dbo.GeoStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GeoCountryId = c.Int(nullable: false),
                        Abbreviation = c.String(nullable: false, maxLength: 5),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeoCountries", t => t.GeoCountryId)
                .Index(t => t.GeoCountryId);
            
            CreateTable(
                "dbo.GeoCountries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GeoCities", "GeoStateId", "dbo.GeoStates");
            DropForeignKey("dbo.GeoStates", "GeoCountryId", "dbo.GeoCountries");
            DropIndex("dbo.GeoStates", new[] { "GeoCountryId" });
            DropIndex("dbo.GeoCities", new[] { "GeoStateId" });
            DropTable("dbo.GeoCountries");
            DropTable("dbo.GeoStates");
            DropTable("dbo.GeoCities");
        }
    }
}
