namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "Points", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "Points");
        }
    }
}
