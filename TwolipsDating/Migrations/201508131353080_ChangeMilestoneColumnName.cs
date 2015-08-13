namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMilestoneColumnName : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Milestones", "UX_PointsAndType");
            AddColumn("dbo.Milestones", "AmountRequired", c => c.Int(nullable: false));
            CreateIndex("dbo.Milestones", new[] { "AmountRequired", "MilestoneTypeId" }, unique: true, name: "UX_AmountAndType");
            DropColumn("dbo.Milestones", "PointsRequired");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Milestones", "PointsRequired", c => c.Int(nullable: false));
            DropIndex("dbo.Milestones", "UX_AmountAndType");
            DropColumn("dbo.Milestones", "AmountRequired");
            CreateIndex("dbo.Milestones", new[] { "PointsRequired", "MilestoneTypeId" }, unique: true, name: "UX_PointsAndType");
        }
    }
}
