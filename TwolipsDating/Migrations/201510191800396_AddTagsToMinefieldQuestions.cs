namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagsToMinefieldQuestions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagMinefieldQuestions",
                c => new
                    {
                        Tag_TagId = c.Int(nullable: false),
                        MinefieldQuestion_MinefieldQuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_TagId, t.MinefieldQuestion_MinefieldQuestionId })
                .ForeignKey("dbo.Tags", t => t.Tag_TagId)
                .ForeignKey("dbo.MinefieldQuestions", t => t.MinefieldQuestion_MinefieldQuestionId)
                .Index(t => t.Tag_TagId)
                .Index(t => t.MinefieldQuestion_MinefieldQuestionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagMinefieldQuestions", "MinefieldQuestion_MinefieldQuestionId", "dbo.MinefieldQuestions");
            DropForeignKey("dbo.TagMinefieldQuestions", "Tag_TagId", "dbo.Tags");
            DropIndex("dbo.TagMinefieldQuestions", new[] { "MinefieldQuestion_MinefieldQuestionId" });
            DropIndex("dbo.TagMinefieldQuestions", new[] { "Tag_TagId" });
            DropTable("dbo.TagMinefieldQuestions");
        }
    }
}
