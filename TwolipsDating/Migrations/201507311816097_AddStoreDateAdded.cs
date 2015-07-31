namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreDateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreItems", "DateAdded", c => c.DateTime(nullable: false, defaultValueSql: "getdate()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreItems", "DateAdded");
        }
    }
}
