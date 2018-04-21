namespace InternDiary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequireSkillText : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Skills", "Text", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Skills", "Text", c => c.String());
        }
    }
}
