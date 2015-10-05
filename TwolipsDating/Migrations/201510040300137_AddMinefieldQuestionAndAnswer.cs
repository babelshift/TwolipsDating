namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinefieldQuestionAndAnswer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinefieldQuestions",
                c => new
                    {
                        MinefieldQuestionId = c.Int(nullable: false),
                        Content = c.String(nullable: false, maxLength: 1000),
                        Points = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MinefieldQuestionId)
                .ForeignKey("dbo.Quizs", t => t.MinefieldQuestionId)
                .Index(t => t.MinefieldQuestionId);
            
            CreateTable(
                "dbo.MinefieldAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinefieldQuestionId = c.Int(nullable: false),
                        Content = c.String(nullable: false, maxLength: 1000),
                        IsCorrect = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MinefieldQuestions", t => t.MinefieldQuestionId)
                .Index(t => t.MinefieldQuestionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinefieldQuestions", "MinefieldQuestionId", "dbo.Quizs");
            DropForeignKey("dbo.MinefieldAnswers", "MinefieldQuestionId", "dbo.MinefieldQuestions");
            DropIndex("dbo.MinefieldAnswers", new[] { "MinefieldQuestionId" });
            DropIndex("dbo.MinefieldQuestions", new[] { "MinefieldQuestionId" });
            DropTable("dbo.MinefieldAnswers");
            DropTable("dbo.MinefieldQuestions");
        }
    }
}
