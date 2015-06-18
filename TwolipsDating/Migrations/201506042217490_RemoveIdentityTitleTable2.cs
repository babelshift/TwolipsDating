namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIdentityTitleTable2 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Titles");

            CreateTable(
                "dbo.Titles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: false),
                    Name = c.String(nullable: false, maxLength: 64),
                    Description = c.String(nullable: false, maxLength: 255),
                    PointPrice = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.Titles");

            CreateTable(
                   "dbo.Titles",
                   c => new
                   {
                       Id = c.Int(nullable: false, identity: true),
                       Name = c.String(nullable: false, maxLength: 64),
                       Description = c.String(nullable: false, maxLength: 255),
                       PointPrice = c.Int(nullable: false),
                   })
                   .PrimaryKey(t => t.Id);
        }
    }
}
