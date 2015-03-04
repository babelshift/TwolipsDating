namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewAndRatingEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorUserId = c.String(nullable: false, maxLength: 128),
                        TargetUserId = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        Content = c.String(nullable: false, maxLength: 2000),
                        RatingValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorUserId)
                .ForeignKey("dbo.ReviewRatings", t => t.RatingValue)
                .ForeignKey("dbo.AspNetUsers", t => t.TargetUserId)
                .Index(t => new { t.AuthorUserId, t.TargetUserId }, unique: true, name: "UX_AuthorAndTarget")
                .Index(t => t.RatingValue);
            
            CreateTable(
                "dbo.ReviewRatings",
                c => new
                    {
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Value);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "TargetUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "RatingValue", "dbo.ReviewRatings");
            DropForeignKey("dbo.Reviews", "AuthorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Reviews", new[] { "RatingValue" });
            DropIndex("dbo.Reviews", "UX_AuthorAndTarget");
            DropTable("dbo.ReviewRatings");
            DropTable("dbo.Reviews");
        }
    }
}
