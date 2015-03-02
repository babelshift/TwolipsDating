namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableZipCodeOnProfile : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Profiles", "ZipCode", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profiles", "ZipCode", c => c.Int(nullable: false));
        }
    }
}
