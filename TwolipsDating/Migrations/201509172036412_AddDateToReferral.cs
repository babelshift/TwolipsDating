namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateToReferral : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Referrals", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Referrals", "DateCreated");
        }
    }
}
