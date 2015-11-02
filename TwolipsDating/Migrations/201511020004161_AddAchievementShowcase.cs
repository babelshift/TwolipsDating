namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAchievementShowcase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MilestoneAchievements", "ShowInAchievementShowcase", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MilestoneAchievements", "ShowInAchievementShowcase");
        }
    }
}
