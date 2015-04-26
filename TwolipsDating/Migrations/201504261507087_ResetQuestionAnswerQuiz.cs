namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetQuestionAnswerQuiz : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "QuizId", "dbo.Quizzes");
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropIndex("dbo.Answers", "UX_QuestionAndCorrect");
            DropIndex("dbo.Questions", new[] { "QuizId" });
            DropTable("dbo.Answers");
            DropTable("dbo.Questions");
            DropTable("dbo.Quizzes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Quizzes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(nullable: false, maxLength: 255),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false, maxLength: 1000),
                        QuizId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false, maxLength: 1000),
                        QuestionId = c.Int(nullable: false),
                        IsCorrect = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Questions", "QuizId");
            CreateIndex("dbo.Answers", new[] { "QuestionId", "IsCorrect" }, unique: true, name: "UX_QuestionAndCorrect");
            AddForeignKey("dbo.Answers", "QuestionId", "dbo.Questions", "Id");
            AddForeignKey("dbo.Questions", "QuizId", "dbo.Quizzes", "Id");
        }
    }
}
