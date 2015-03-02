namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedCityNameUniqueIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Cities", "UX_Name");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Cities", "Name", unique: true, name: "UX_Name");
        }
    }
}
