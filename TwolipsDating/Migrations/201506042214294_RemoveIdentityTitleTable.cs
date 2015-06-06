namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIdentityTitleTable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Titles");
            AlterColumn("dbo.Titles", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Titles", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Titles");
            AlterColumn("dbo.Titles", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Titles", "Id");
        }
    }
}
