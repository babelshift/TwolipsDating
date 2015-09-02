namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileSummaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "SummaryOfSelf", c => c.String(maxLength: 500));
            AddColumn("dbo.Profiles", "SummaryOfDoing", c => c.String(maxLength: 500));
            AddColumn("dbo.Profiles", "SummaryOfGoing", c => c.String(maxLength: 500));
            DropColumn("dbo.Profiles", "SelfDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Profiles", "SelfDescription", c => c.String(maxLength: 200));
            DropColumn("dbo.Profiles", "SummaryOfGoing");
            DropColumn("dbo.Profiles", "SummaryOfDoing");
            DropColumn("dbo.Profiles", "SummaryOfSelf");
        }
    }
}
