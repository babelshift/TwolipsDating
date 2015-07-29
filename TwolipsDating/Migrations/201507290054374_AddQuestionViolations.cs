namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuestionViolations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionViolations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorUserId = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        QuestionViolationTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorUserId)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.QuestionViolationTypes", t => t.QuestionViolationTypeId)
                .Index(t => t.AuthorUserId)
                .Index(t => t.QuestionId)
                .Index(t => t.QuestionViolationTypeId);
            
            CreateTable(
                "dbo.QuestionViolationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionViolations", "QuestionViolationTypeId", "dbo.QuestionViolationTypes");
            DropForeignKey("dbo.QuestionViolations", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.QuestionViolations", "AuthorUserId", "dbo.AspNetUsers");
            DropIndex("dbo.QuestionViolationTypes", "UX_Name");
            DropIndex("dbo.QuestionViolations", new[] { "QuestionViolationTypeId" });
            DropIndex("dbo.QuestionViolations", new[] { "QuestionId" });
            DropIndex("dbo.QuestionViolations", new[] { "AuthorUserId" });
            DropTable("dbo.QuestionViolationTypes");
            DropTable("dbo.QuestionViolations");
        }
    }
}
