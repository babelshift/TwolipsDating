namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionAnswerTracking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnsweredQuestions",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        QuestionId = c.Int(nullable: false),
                        AnswerId = c.Int(nullable: false),
                        DateAnswered = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.QuestionId })
                .ForeignKey("dbo.Answers", t => t.AnswerId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.QuestionId)
                .Index(t => t.AnswerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnsweredQuestions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AnsweredQuestions", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.AnsweredQuestions", "AnswerId", "dbo.Answers");
            DropIndex("dbo.AnsweredQuestions", new[] { "AnswerId" });
            DropIndex("dbo.AnsweredQuestions", new[] { "QuestionId" });
            DropIndex("dbo.AnsweredQuestions", new[] { "UserId" });
            DropTable("dbo.AnsweredQuestions");
        }
    }
}
