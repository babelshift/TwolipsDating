namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLifetimePoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LifetimePoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LifetimePoints");
        }
    }
}
