namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIdentityTitleTable2 : DbMigration
    {
        public override void Up()
        {
            this.ChangeIdentity(IdentityChange.SwitchIdentityOff, "dbo.Titles", "Id");
        }
        
        public override void Down()
        {
            this.ChangeIdentity(IdentityChange.SwitchIdentityOn, "dbo.Titles", "Id");
        }
    }
}
