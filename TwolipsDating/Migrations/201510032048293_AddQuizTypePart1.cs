namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizTypePart1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuizTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Quizs", "QuizTypeId", c => c.Int());
            CreateIndex("dbo.Quizs", "QuizTypeId");
            AddForeignKey("dbo.Quizs", "QuizTypeId", "dbo.QuizTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quizs", "QuizTypeId", "dbo.QuizTypes");
            DropIndex("dbo.Quizs", new[] { "QuizTypeId" });
            DropColumn("dbo.Quizs", "QuizTypeId");
            DropTable("dbo.QuizTypes");
        }
    }
}
