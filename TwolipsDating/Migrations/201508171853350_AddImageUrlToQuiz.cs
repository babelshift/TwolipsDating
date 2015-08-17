namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageUrlToQuiz : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "ImageUrl");
        }
    }
}
