namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMilestoneIdentity : DbMigration
    {
        public override void Up()
        {
            Sql("delete from dbo.MilestoneAchievements");

            DropForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones");

            DropTable("dbo.Milestones");

            CreateTable(
                "dbo.Milestones",
                c => new
                {
                    Id = c.Int(nullable: false, identity: false),
                    AmountRequired = c.Int(nullable: false),
                    MilestoneTypeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MilestoneTypes", t => t.MilestoneTypeId)
                .Index(t => new { t.AmountRequired, t.MilestoneTypeId }, unique: true, name: "UX_AmountAndType");

            AddForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones");

            DropTable("dbo.Milestones");

            CreateTable(
                "dbo.Milestones",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    AmountRequired = c.Int(nullable: false),
                    MilestoneTypeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MilestoneTypes", t => t.MilestoneTypeId)
                .Index(t => new { t.AmountRequired, t.MilestoneTypeId }, unique: true, name: "UX_AmountAndType");

            AddForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones", "Id");
        }
    }
}
