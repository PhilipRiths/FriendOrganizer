namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meeting", "Location", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meeting", "Location");
        }
    }
}
