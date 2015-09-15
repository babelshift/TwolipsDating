namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIconToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuizCategories", "FontAwesomeIconName", c => c.String(nullable: false, maxLength: 50, defaultValue: "fa-question"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QuizCategories", "FontAwesomeIconName");
        }
    }
}
