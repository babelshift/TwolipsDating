namespace TwolipsDating.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AddTagTable : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.Tags",
				c => new
					{
						TagId = c.Int(nullable: false),
						Name = c.String(nullable: false, maxLength: 255),
						Description = c.String(),
					})
				.PrimaryKey(t => t.TagId)
				.Index(t => t.Name, unique: true, name: "UX_Name");

			CreateTable(
				"dbo.TagProfiles",
				c => new
					{
						Tag_TagId = c.Int(nullable: false),
						Profile_Id = c.Int(nullable: false),
					})
				.PrimaryKey(t => new { t.Tag_TagId, t.Profile_Id })
				.ForeignKey("dbo.Tags", t => t.Tag_TagId)
				.ForeignKey("dbo.Profiles", t => t.Profile_Id)
				.Index(t => new { t.Tag_TagId, t.Profile_Id }, unique: true, name: "UX_TagAndProfile");

		}

		public override void Down()
		{
			DropForeignKey("dbo.TagProfiles", "Profile_Id", "dbo.Profiles");
			DropForeignKey("dbo.TagProfiles", "Tag_TagId", "dbo.Tags");
			DropIndex("dbo.TagProfiles", "UX_TagAndProfile");
			DropIndex("dbo.Tags", "UX_Name");
			DropTable("dbo.TagProfiles");
			DropTable("dbo.Tags");
		}
	}
}
