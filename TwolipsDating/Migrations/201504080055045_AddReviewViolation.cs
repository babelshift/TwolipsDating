namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewViolation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReviewViolations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorUserId = c.String(nullable: false, maxLength: 128),
                        Content = c.String(nullable: false, maxLength: 3000),
                        DateCreated = c.DateTime(nullable: false),
                        ReviewId = c.Int(nullable: false),
                        ViolationTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorUserId)
                .ForeignKey("dbo.Reviews", t => t.ReviewId)
                .ForeignKey("dbo.ViolationTypes", t => t.ViolationTypeId)
                .Index(t => t.AuthorUserId)
                .Index(t => t.ReviewId)
                .Index(t => t.ViolationTypeId);
            
            CreateTable(
                "dbo.ViolationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReviewViolations", "ViolationTypeId", "dbo.ViolationTypes");
            DropForeignKey("dbo.ReviewViolations", "ReviewId", "dbo.Reviews");
            DropForeignKey("dbo.ReviewViolations", "AuthorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ViolationTypes", "UX_Name");
            DropIndex("dbo.ReviewViolations", new[] { "ViolationTypeId" });
            DropIndex("dbo.ReviewViolations", new[] { "ReviewId" });
            DropIndex("dbo.ReviewViolations", new[] { "AuthorUserId" });
            DropTable("dbo.ViolationTypes");
            DropTable("dbo.ReviewViolations");
        }
    }
}
