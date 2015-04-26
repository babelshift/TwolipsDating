namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingQuestionAnswerRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false, maxLength: 1000),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false, maxLength: 1000),
                        QuizId = c.Int(),
                        CorrectAnswerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Answers", t => t.CorrectAnswerId)
                .ForeignKey("dbo.Quizs", t => t.QuizId)
                .Index(t => t.QuizId)
                .Index(t => t.CorrectAnswerId);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(nullable: false, maxLength: 255),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Answers", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Questions", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.Questions", "CorrectAnswerId", "dbo.Answers");
            DropIndex("dbo.Questions", new[] { "CorrectAnswerId" });
            DropIndex("dbo.Questions", new[] { "QuizId" });
            DropIndex("dbo.Answers", new[] { "QuestionId" });
            DropTable("dbo.Quizs");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
