namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStateAndCityChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.USStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Abbreviation = c.String(nullable: false, maxLength: 2),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Abbreviation, unique: true, name: "UX_Abbreviation")
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
            AddColumn("dbo.Cities", "USStateId", c => c.Int());
            AlterColumn("dbo.Cities", "ZipCode", c => c.Int());
            CreateIndex("dbo.Cities", "USStateId");
            AddForeignKey("dbo.Cities", "USStateId", "dbo.USStates", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cities", "USStateId", "dbo.USStates");
            DropIndex("dbo.USStates", "UX_Name");
            DropIndex("dbo.USStates", "UX_Abbreviation");
            DropIndex("dbo.Cities", new[] { "USStateId" });
            AlterColumn("dbo.Cities", "ZipCode", c => c.Int(nullable: false));
            DropColumn("dbo.Cities", "USStateId");
            DropTable("dbo.USStates");
        }
    }
}
