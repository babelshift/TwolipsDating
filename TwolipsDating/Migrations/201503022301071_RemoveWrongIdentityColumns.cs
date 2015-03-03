namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveWrongIdentityColumns : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "MessageStatusId", "dbo.MessageStatus");
            DropForeignKey("dbo.Profiles", "GenderId", "dbo.Genders");
            //DropPrimaryKey("dbo.MessageStatus");
            //DropPrimaryKey("dbo.Genders");
            this.ChangeIdentity(IdentityChange.SwitchIdentityOff, "dbo.MessageStatus", "Id");
            this.ChangeIdentity(IdentityChange.SwitchIdentityOff, "dbo.Genders", "Id");
            //AddPrimaryKey("dbo.MessageStatus", "Id");
            //AddPrimaryKey("dbo.Genders", "Id");
            AddForeignKey("dbo.Messages", "MessageStatusId", "dbo.MessageStatus", "Id");
            AddForeignKey("dbo.Profiles", "GenderId", "dbo.Genders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "GenderId", "dbo.Genders");
            DropForeignKey("dbo.Messages", "MessageStatusId", "dbo.MessageStatus");
            //DropPrimaryKey("dbo.Genders");
            //DropPrimaryKey("dbo.MessageStatus");
            this.ChangeIdentity(IdentityChange.SwitchIdentityOn, "dbo.MessageStatus", "Id");
            this.ChangeIdentity(IdentityChange.SwitchIdentityOn, "dbo.Genders", "Id");
            //AddPrimaryKey("dbo.Genders", "Id");
            //AddPrimaryKey("dbo.MessageStatus", "Id");
            AddForeignKey("dbo.Profiles", "GenderId", "dbo.Genders", "Id");
            AddForeignKey("dbo.Messages", "MessageStatusId", "dbo.MessageStatus", "Id");
        }
    }
}
