namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixScrewedUpUserToProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "ApplicationUserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "ApplicationUserId");
        }
    }
}
