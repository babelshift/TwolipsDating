namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlankForProfileUniqueUser : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Profiles", "ApplicationUser_Id", unique: true, name: "UX_ApplicationUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Profiles", "ApplicationUser_Id");
        }
    }
}
