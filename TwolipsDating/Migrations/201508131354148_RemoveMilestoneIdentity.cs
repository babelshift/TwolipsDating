namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMilestoneIdentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones");
            this.ChangeIdentity(IdentityChange.SwitchIdentityOff, "dbo.Milestones", "Id");
            AddForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones");
            this.ChangeIdentity(IdentityChange.SwitchIdentityOn, "dbo.Milestones", "Id");
            AddForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones", "Id");
        }
    }
}
