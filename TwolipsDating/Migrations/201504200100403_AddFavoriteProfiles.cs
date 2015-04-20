namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavoriteProfiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavoriteProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ProfileId = c.Int(nullable: false),
                        DateFavorited = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ProfileId })
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavoriteProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.FavoriteProfiles", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.FavoriteProfiles", new[] { "ProfileId" });
            DropIndex("dbo.FavoriteProfiles", new[] { "UserId" });
            DropTable("dbo.FavoriteProfiles");
        }
    }
}
