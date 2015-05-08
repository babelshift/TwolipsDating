namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTagAwardDate : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TagAwards");
            AddColumn("dbo.TagAwards", "DateAwarded", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.TagAwards", new[] { "TagId", "ProfileId", "DateAwarded" });
            DropColumn("dbo.TagAwards", "DateSuggested");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TagAwards", "DateSuggested", c => c.DateTime(nullable: false));
            DropPrimaryKey("dbo.TagAwards");
            DropColumn("dbo.TagAwards", "DateAwarded");
            AddPrimaryKey("dbo.TagAwards", new[] { "TagId", "ProfileId", "DateSuggested" });
        }
    }
}
