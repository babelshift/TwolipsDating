namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeQuizAsOptional : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Questions", new[] { "QuizId" });
            AlterColumn("dbo.Questions", "QuizId", c => c.Int());
            CreateIndex("dbo.Questions", "QuizId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Questions", new[] { "QuizId" });
            AlterColumn("dbo.Questions", "QuizId", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "QuizId");
        }
    }
}
