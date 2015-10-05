namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizTypePart2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Quizs", new[] { "QuizTypeId" });
            AlterColumn("dbo.Quizs", "QuizTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Quizs", "QuizTypeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Quizs", new[] { "QuizTypeId" });
            AlterColumn("dbo.Quizs", "QuizTypeId", c => c.Int());
            CreateIndex("dbo.Quizs", "QuizTypeId");
        }
    }
}
