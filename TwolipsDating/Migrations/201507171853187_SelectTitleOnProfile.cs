namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SelectTitleOnProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "SelectedTitleId", c => c.Int());
            CreateIndex("dbo.Profiles", "SelectedTitleId");
            AddForeignKey("dbo.Profiles", "SelectedTitleId", "dbo.Titles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "SelectedTitleId", "dbo.Titles");
            DropIndex("dbo.Profiles", new[] { "SelectedTitleId" });
            DropColumn("dbo.Profiles", "SelectedTitleId");
        }
    }
}
