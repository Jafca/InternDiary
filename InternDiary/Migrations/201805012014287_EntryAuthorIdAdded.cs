namespace InternDiary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntryAuthorIdAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entries", "AuthorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Entries", "AuthorId");
            AddForeignKey("dbo.Entries", "AuthorId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entries", "AuthorId", "dbo.AspNetUsers");
            DropIndex("dbo.Entries", new[] { "AuthorId" });
            DropColumn("dbo.Entries", "AuthorId");
        }
    }
}
