namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedSelfDescriptionRequirement : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Profiles", "SelfDescription", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profiles", "SelfDescription", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
