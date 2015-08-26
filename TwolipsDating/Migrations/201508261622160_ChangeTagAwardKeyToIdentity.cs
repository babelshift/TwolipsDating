namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTagAwardKeyToIdentity : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TagAwards");
            AddColumn("dbo.TagAwards", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TagAwards", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TagAwards");
            DropColumn("dbo.TagAwards", "Id");
            AddPrimaryKey("dbo.TagAwards", new[] { "TagId", "ProfileId", "DateAwarded" });
        }
    }
}
