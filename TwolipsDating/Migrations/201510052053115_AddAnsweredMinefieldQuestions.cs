namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnsweredMinefieldQuestions : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.QuizQuestions", newName: "QuestionQuizs");
            DropPrimaryKey("dbo.QuestionQuizs");
            CreateTable(
                "dbo.AnsweredMinefieldQuestions",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        MinefieldAnswerId = c.Int(nullable: false),
                        MinefieldQuestionId = c.Int(nullable: false),
                        DateAnswered = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.MinefieldAnswerId })
                .ForeignKey("dbo.MinefieldAnswers", t => t.MinefieldAnswerId)
                .ForeignKey("dbo.MinefieldQuestions", t => t.MinefieldQuestionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.MinefieldAnswerId)
                .Index(t => t.MinefieldQuestionId);
            
            AddPrimaryKey("dbo.QuestionQuizs", new[] { "Question_Id", "Quiz_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnsweredMinefieldQuestions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AnsweredMinefieldQuestions", "MinefieldQuestionId", "dbo.MinefieldQuestions");
            DropForeignKey("dbo.AnsweredMinefieldQuestions", "MinefieldAnswerId", "dbo.MinefieldAnswers");
            DropIndex("dbo.AnsweredMinefieldQuestions", new[] { "MinefieldQuestionId" });
            DropIndex("dbo.AnsweredMinefieldQuestions", new[] { "MinefieldAnswerId" });
            DropIndex("dbo.AnsweredMinefieldQuestions", new[] { "UserId" });
            DropPrimaryKey("dbo.QuestionQuizs");
            DropTable("dbo.AnsweredMinefieldQuestions");
            AddPrimaryKey("dbo.QuestionQuizs", new[] { "Quiz_Id", "Question_Id" });
            RenameTable(name: "dbo.QuestionQuizs", newName: "QuizQuestions");
        }
    }
}
