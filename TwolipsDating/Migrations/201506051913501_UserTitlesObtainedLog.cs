namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTitlesObtainedLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTitles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TitleId = c.Int(nullable: false),
                        DateObtained = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.TitleId })
                .ForeignKey("dbo.Titles", t => t.TitleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.TitleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTitles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserTitles", "TitleId", "dbo.Titles");
            DropIndex("dbo.UserTitles", new[] { "TitleId" });
            DropIndex("dbo.UserTitles", new[] { "UserId" });
            DropTable("dbo.UserTitles");
        }
    }
}
