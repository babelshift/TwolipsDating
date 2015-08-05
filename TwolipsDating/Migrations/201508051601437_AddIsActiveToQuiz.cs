namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActiveToQuiz : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "IsActive");
        }
    }
}
