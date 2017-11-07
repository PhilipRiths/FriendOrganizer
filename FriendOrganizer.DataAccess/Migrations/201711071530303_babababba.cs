namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class babababba : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Friend", name: "FavouriteLanguageId", newName: "FavouriteLanguage_Id");
            RenameIndex(table: "dbo.Friend", name: "IX_FavouriteLanguageId", newName: "IX_FavouriteLanguage_Id");
            AddColumn("dbo.Friend", "FavoriteLanguageId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Friend", "FavoriteLanguageId");
            RenameIndex(table: "dbo.Friend", name: "IX_FavouriteLanguage_Id", newName: "IX_FavouriteLanguageId");
            RenameColumn(table: "dbo.Friend", name: "FavouriteLanguage_Id", newName: "FavouriteLanguageId");
        }
    }
}
