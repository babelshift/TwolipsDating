namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookingForLocations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Range = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookingForTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RelationshipStatus",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LanguageProfiles",
                c => new
                    {
                        Language_Id = c.Int(nullable: false),
                        Profile_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Language_Id, t.Profile_Id })
                .ForeignKey("dbo.Languages", t => t.Language_Id)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .Index(t => t.Language_Id)
                .Index(t => t.Profile_Id);
            
            AddColumn("dbo.Profiles", "LookingForAgeMin", c => c.Int());
            AddColumn("dbo.Profiles", "LookingForAgeMax", c => c.Int());
            AddColumn("dbo.Profiles", "LookingForTypeId", c => c.Int());
            AddColumn("dbo.Profiles", "RelationshipStatusId", c => c.Int());
            AddColumn("dbo.Profiles", "LookingForLocationId", c => c.Int());
            CreateIndex("dbo.Profiles", "LookingForTypeId");
            CreateIndex("dbo.Profiles", "RelationshipStatusId");
            CreateIndex("dbo.Profiles", "LookingForLocationId");
            AddForeignKey("dbo.Profiles", "LookingForLocationId", "dbo.LookingForLocations", "Id");
            AddForeignKey("dbo.Profiles", "LookingForTypeId", "dbo.LookingForTypes", "Id");
            AddForeignKey("dbo.Profiles", "RelationshipStatusId", "dbo.RelationshipStatus", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "RelationshipStatusId", "dbo.RelationshipStatus");
            DropForeignKey("dbo.Profiles", "LookingForTypeId", "dbo.LookingForTypes");
            DropForeignKey("dbo.Profiles", "LookingForLocationId", "dbo.LookingForLocations");
            DropForeignKey("dbo.LanguageProfiles", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.LanguageProfiles", "Language_Id", "dbo.Languages");
            DropIndex("dbo.LanguageProfiles", new[] { "Profile_Id" });
            DropIndex("dbo.LanguageProfiles", new[] { "Language_Id" });
            DropIndex("dbo.Profiles", new[] { "LookingForLocationId" });
            DropIndex("dbo.Profiles", new[] { "RelationshipStatusId" });
            DropIndex("dbo.Profiles", new[] { "LookingForTypeId" });
            DropColumn("dbo.Profiles", "LookingForLocationId");
            DropColumn("dbo.Profiles", "RelationshipStatusId");
            DropColumn("dbo.Profiles", "LookingForTypeId");
            DropColumn("dbo.Profiles", "LookingForAgeMax");
            DropColumn("dbo.Profiles", "LookingForAgeMin");
            DropTable("dbo.LanguageProfiles");
            DropTable("dbo.RelationshipStatus");
            DropTable("dbo.LookingForTypes");
            DropTable("dbo.LookingForLocations");
            DropTable("dbo.Languages");
        }
    }
}
