namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReferrals : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Referrals",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IsRedeemed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Referrals", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Referrals", new[] { "UserId" });
            DropTable("dbo.Referrals");
        }
    }
}
