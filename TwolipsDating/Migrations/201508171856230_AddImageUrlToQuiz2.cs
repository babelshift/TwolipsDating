namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageUrlToQuiz2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "ImageFileName", c => c.String());
            DropColumn("dbo.Quizs", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Quizs", "ImageUrl", c => c.String());
            DropColumn("dbo.Quizs", "ImageFileName");
        }
    }
}
