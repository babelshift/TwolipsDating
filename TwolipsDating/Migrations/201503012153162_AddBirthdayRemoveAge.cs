namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBirthdayRemoveAge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "Birthday", c => c.DateTime(nullable: false));
            DropColumn("dbo.Profiles", "Age");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Profiles", "Age", c => c.Int(nullable: false));
            DropColumn("dbo.Profiles", "Birthday");
        }
    }
}
