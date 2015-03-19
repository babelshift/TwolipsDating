namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagSuggestions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagSuggestions",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        ProfileId = c.Int(nullable: false),
                        SuggestingUserId = c.String(nullable: false, maxLength: 128),
                        DateSuggested = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagId, t.ProfileId, t.SuggestingUserId })
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.AspNetUsers", t => t.SuggestingUserId)
                .ForeignKey("dbo.Tags", t => t.TagId)
                .Index(t => t.TagId)
                .Index(t => t.ProfileId)
                .Index(t => t.SuggestingUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagSuggestions", "TagId", "dbo.Tags");
            DropForeignKey("dbo.TagSuggestions", "SuggestingUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TagSuggestions", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.TagSuggestions", new[] { "SuggestingUserId" });
            DropIndex("dbo.TagSuggestions", new[] { "ProfileId" });
            DropIndex("dbo.TagSuggestions", new[] { "TagId" });
            DropTable("dbo.TagSuggestions");
        }
    }
}
