namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeKeyOnProfileViewLog : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ProfileViewLogs");
            AddPrimaryKey("dbo.ProfileViewLogs", new[] { "ViewerUserId", "TargetProfileId", "DateVisited" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ProfileViewLogs");
            AddPrimaryKey("dbo.ProfileViewLogs", new[] { "ViewerUserId", "TargetProfileId" });
        }
    }
}
