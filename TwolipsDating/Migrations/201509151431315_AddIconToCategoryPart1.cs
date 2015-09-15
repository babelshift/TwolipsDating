namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIconToCategoryPart1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuizCategories", "FontAwesomeIconName", c => c.String(maxLength: 50, defaultValue: "fa-question"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuizCategories", "FontAwesomeIconName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
