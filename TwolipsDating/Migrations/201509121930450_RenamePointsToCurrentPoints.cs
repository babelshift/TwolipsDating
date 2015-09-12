namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePointsToCurrentPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CurrentPoints", c => c.Int(nullable: false));
            Sql("update dbo.AspNetUsers set CurrentPoints = Points");
            DropColumn("dbo.AspNetUsers", "Points");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Points", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "CurrentPoints");
        }
    }
}
