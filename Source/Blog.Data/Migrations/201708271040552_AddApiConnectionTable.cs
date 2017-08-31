namespace Blog.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddApiConnectionTable : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ApiConnections",
                c => new
                    {
                        Id = c.Int(false, true),
                        ApiName = c.String(false),
                        Key = c.String(),
                        Secrete = c.String(),
                        Token = c.String()
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            this.DropTable("dbo.ApiConnections");
        }
    }
}