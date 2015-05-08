namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Milestones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MilestoneAchievements",
                c => new
                    {
                        MilestoneId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        DateAchieved = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MilestoneId, t.UserId })
                .ForeignKey("dbo.Milestones", t => t.MilestoneId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MilestoneId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Milestones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PointsRequired = c.Int(nullable: false),
                        MilestoneTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MilestoneTypes", t => t.MilestoneTypeId)
                .Index(t => new { t.PointsRequired, t.MilestoneTypeId }, unique: true, name: "UX_PointsAndType");
            
            CreateTable(
                "dbo.MilestoneTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MilestoneAchievements", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MilestoneAchievements", "MilestoneId", "dbo.Milestones");
            DropForeignKey("dbo.Milestones", "MilestoneTypeId", "dbo.MilestoneTypes");
            DropIndex("dbo.MilestoneTypes", "UX_Name");
            DropIndex("dbo.Milestones", "UX_PointsAndType");
            DropIndex("dbo.MilestoneAchievements", new[] { "UserId" });
            DropIndex("dbo.MilestoneAchievements", new[] { "MilestoneId" });
            DropTable("dbo.MilestoneTypes");
            DropTable("dbo.Milestones");
            DropTable("dbo.MilestoneAchievements");
        }
    }
}
