namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagAwards : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagAwards",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        ProfileId = c.Int(nullable: false),
                        DateSuggested = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagId, t.ProfileId, t.DateSuggested })
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.Tags", t => t.TagId)
                .Index(t => t.TagId)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagAwards", "TagId", "dbo.Tags");
            DropForeignKey("dbo.TagAwards", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.TagAwards", new[] { "ProfileId" });
            DropIndex("dbo.TagAwards", new[] { "TagId" });
            DropTable("dbo.TagAwards");
        }
    }
}
