namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionsAndTagsRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagQuestions",
                c => new
                    {
                        Tag_TagId = c.Int(nullable: false),
                        Question_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_TagId, t.Question_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_TagId)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Tag_TagId)
                .Index(t => t.Question_Id);
            
            AddColumn("dbo.Questions", "Points", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagQuestions", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.TagQuestions", "Tag_TagId", "dbo.Tags");
            DropIndex("dbo.TagQuestions", new[] { "Question_Id" });
            DropIndex("dbo.TagQuestions", new[] { "Tag_TagId" });
            DropColumn("dbo.Questions", "Points");
            DropTable("dbo.TagQuestions");
        }
    }
}
