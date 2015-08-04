namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveQuizIdentity : DbMigration
    {
        public override void Up()
        {
            Sql("delete from dbo.CompletedQuizs");
            Sql("delete from dbo.QuizQuestions");

            DropForeignKey("dbo.QuizQuestions", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.CompletedQuizs", "QuizId", "dbo.Quizs");

            DropTable("dbo.Quizs");

            CreateTable(
                "dbo.Quizs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: false),
                    Name = c.String(nullable: false, maxLength: 64),
                    Description = c.String(nullable: false, maxLength: 255),
                    DateCreated = c.DateTime(nullable: false),
                    Points = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.Id);

            AddForeignKey("dbo.QuizQuestions", "Quiz_Id", "dbo.Quizs", "Id");
            AddForeignKey("dbo.CompletedQuizs", "QuizId", "dbo.Quizs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedQuizs", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.QuizQuestions", "Quiz_Id", "dbo.Quizs");

            DropTable("dbo.Quizs");

            CreateTable(
                "dbo.Quizs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 64),
                    Description = c.String(nullable: false, maxLength: 255),
                    DateCreated = c.DateTime(nullable: false),
                    Points = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.Id);

            AddForeignKey("dbo.CompletedQuizs", "QuizId", "dbo.Quizs", "Id");
            AddForeignKey("dbo.QuizQuestions", "Quiz_Id", "dbo.Quizs", "Id");
        }
    }
}
