namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIgnoredUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IgnoredUsers",
                c => new
                    {
                        SourceUserId = c.String(nullable: false, maxLength: 128),
                        TargetUserId = c.String(nullable: false, maxLength: 128),
                        DateIgnored = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.SourceUserId, t.TargetUserId })
                .ForeignKey("dbo.AspNetUsers", t => t.SourceUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.TargetUserId)
                .Index(t => t.SourceUserId)
                .Index(t => t.TargetUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IgnoredUsers", "TargetUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.IgnoredUsers", "SourceUserId", "dbo.AspNetUsers");
            DropIndex("dbo.IgnoredUsers", new[] { "TargetUserId" });
            DropIndex("dbo.IgnoredUsers", new[] { "SourceUserId" });
            DropTable("dbo.IgnoredUsers");
        }
    }
}
