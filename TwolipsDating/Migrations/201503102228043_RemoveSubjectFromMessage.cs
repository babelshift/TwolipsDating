namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSubjectFromMessage : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Messages", "Subject");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "Subject", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
