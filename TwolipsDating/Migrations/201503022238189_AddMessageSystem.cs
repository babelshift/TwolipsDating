namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageSystem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderApplicationUserId = c.String(nullable: false, maxLength: 128),
                        ReceiverApplicationUserId = c.String(nullable: false, maxLength: 128),
                        DateSent = c.DateTime(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 255),
                        Body = c.String(nullable: false, maxLength: 2000),
                        MessageStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MessageStatus", t => t.MessageStatusId)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverApplicationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderApplicationUserId)
                .Index(t => t.SenderApplicationUserId)
                .Index(t => t.ReceiverApplicationUserId)
                .Index(t => t.MessageStatusId);
            
            CreateTable(
                "dbo.MessageStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Name");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "SenderApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "ReceiverApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "MessageStatusId", "dbo.MessageStatus");
            DropIndex("dbo.MessageStatus", "UX_Name");
            DropIndex("dbo.Messages", new[] { "MessageStatusId" });
            DropIndex("dbo.Messages", new[] { "ReceiverApplicationUserId" });
            DropIndex("dbo.Messages", new[] { "SenderApplicationUserId" });
            DropTable("dbo.MessageStatus");
            DropTable("dbo.Messages");
        }
    }
}
