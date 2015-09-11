namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizCategoryPart1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuizCategories",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Quizs", "QuizCategoryId", c => c.Int(defaultValue: 0));
            CreateIndex("dbo.Quizs", "QuizCategoryId");
            AddForeignKey("dbo.Quizs", "QuizCategoryId", "dbo.QuizCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quizs", "QuizCategoryId", "dbo.QuizCategories");
            DropIndex("dbo.Quizs", new[] { "QuizCategoryId" });
            DropColumn("dbo.Quizs", "QuizCategoryId");
            DropTable("dbo.QuizCategories");
        }
    }
}
