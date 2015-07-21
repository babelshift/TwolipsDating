namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateProfileToGeoCity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Cities", "USStateId", "dbo.USStates");
            DropForeignKey("dbo.ZipCodes", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Profiles", "CityId", "dbo.Cities");
            DropIndex("dbo.Profiles", new[] { "CityId" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropIndex("dbo.Cities", new[] { "USStateId" });
            DropIndex("dbo.Countries", "UX_Name");
            DropIndex("dbo.USStates", "UX_Abbreviation");
            DropIndex("dbo.USStates", "UX_Name");
            DropIndex("dbo.ZipCodes", new[] { "CityId" });
            Sql("insert into dbo.GeoCountries(name) values('Unknown')");
            Sql("insert into dbo.GeoStates(geocountryid, abbreviation) values(2, 'UNK')");
            Sql("insert into dbo.GeoCities(geostateid, name) values(2, 'Unknown')");
            AddColumn("dbo.Profiles", "GeoCityId", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Profiles", "GeoCityId");
            AddForeignKey("dbo.Profiles", "GeoCityId", "dbo.GeoCities", "Id");
            DropColumn("dbo.Profiles", "ZipCode");
            DropColumn("dbo.Profiles", "CityId");
            DropTable("dbo.Cities");
            DropTable("dbo.Countries");
            DropTable("dbo.USStates");
            DropTable("dbo.ZipCodes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ZipCodes",
                c => new
                    {
                        ZipCodeId = c.String(nullable: false, maxLength: 5),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ZipCodeId);
            
            CreateTable(
                "dbo.USStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Abbreviation = c.String(nullable: false, maxLength: 2),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        CountryId = c.Int(nullable: false),
                        USStateId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Profiles", "CityId", c => c.Int(nullable: false));
            AddColumn("dbo.Profiles", "ZipCode", c => c.Int());
            DropForeignKey("dbo.Profiles", "GeoCityId", "dbo.GeoCities");
            DropIndex("dbo.Profiles", new[] { "GeoCityId" });
            DropColumn("dbo.Profiles", "GeoCityId");
            CreateIndex("dbo.ZipCodes", "CityId");
            CreateIndex("dbo.USStates", "Name", unique: true, name: "UX_Name");
            CreateIndex("dbo.USStates", "Abbreviation", unique: true, name: "UX_Abbreviation");
            CreateIndex("dbo.Countries", "Name", unique: true, name: "UX_Name");
            CreateIndex("dbo.Cities", "USStateId");
            CreateIndex("dbo.Cities", "CountryId");
            CreateIndex("dbo.Profiles", "CityId");
            AddForeignKey("dbo.Profiles", "CityId", "dbo.Cities", "Id");
            AddForeignKey("dbo.ZipCodes", "CityId", "dbo.Cities", "Id");
            AddForeignKey("dbo.Cities", "USStateId", "dbo.USStates", "Id");
            AddForeignKey("dbo.Cities", "CountryId", "dbo.Countries", "Id");
        }
    }
}
