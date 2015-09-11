namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizCategoryPart2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Quizs", new[] { "QuizCategoryId" });
            Sql("update dbo.Quizs set QuizCategoryId = 0 where QuizCategoryId is null");
            AlterColumn("dbo.Quizs", "QuizCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Quizs", "QuizCategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Quizs", new[] { "QuizCategoryId" });
            AlterColumn("dbo.Quizs", "QuizCategoryId", c => c.Int());
            CreateIndex("dbo.Quizs", "QuizCategoryId");
        }
    }
}
