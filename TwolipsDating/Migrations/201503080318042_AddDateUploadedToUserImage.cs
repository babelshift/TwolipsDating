namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateUploadedToUserImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserImages", "DateUploaded", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserImages", "DateUploaded");
        }
    }
}
