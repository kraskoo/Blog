namespace Blog.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserValidationProfilePictureProperty : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "HasOwnProfilePicture", c => c.Boolean(false));
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "HasOwnProfilePicture");
        }
    }
}