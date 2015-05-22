namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompletedQuizzes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedQuizs",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        QuizId = c.Int(nullable: false),
                        DateCompleted = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.QuizId })
                .ForeignKey("dbo.Quizs", t => t.QuizId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.QuizId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedQuizs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompletedQuizs", "QuizId", "dbo.Quizs");
            DropIndex("dbo.CompletedQuizs", new[] { "QuizId" });
            DropIndex("dbo.CompletedQuizs", new[] { "UserId" });
            DropTable("dbo.CompletedQuizs");
        }
    }
}
