namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueKeysToGeoLocations : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.GeoCities", new[] { "GeoStateId" });
            DropIndex("dbo.GeoStates", new[] { "GeoCountryId" });
            CreateIndex("dbo.GeoCities", new[] { "GeoStateId", "Name" }, unique: true, name: "UX_StateAndCity");
            CreateIndex("dbo.GeoStates", new[] { "GeoCountryId", "Abbreviation" }, unique: true, name: "UX_CountryAndState");
        }
        
        public override void Down()
        {
            DropIndex("dbo.GeoStates", "UX_CountryAndState");
            DropIndex("dbo.GeoCities", "UX_StateAndCity");
            CreateIndex("dbo.GeoStates", "GeoCountryId");
            CreateIndex("dbo.GeoCities", "GeoStateId");
        }
    }
}
