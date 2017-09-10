namespace Blog.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddingVoteRepliesTable : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.Votes",
                c => new
                    {
                        Id = c.Int(false, true),
                        Rate = c.Int(false),
                        ReplyId = c.Int(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Replies", t => t.ReplyId, true)
                .Index(t => t.ReplyId);

            this.CreateTable(
                "dbo.UserVotes",
                c => new
                    {
                        UserId = c.String(false, 128),
                        VoteId = c.Int(false),
                    })
                .PrimaryKey(t => new { t.UserId, t.VoteId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, true)
                .ForeignKey("dbo.Votes", t => t.VoteId, true)
                .Index(t => t.UserId)
                .Index(t => t.VoteId);
            
        }
        
        public override void Down()
        {
            this.DropForeignKey("dbo.UserVotes", "VoteId", "dbo.Votes");
            this.DropForeignKey("dbo.UserVotes", "UserId", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.Votes", "ReplyId", "dbo.Replies");
            this.DropIndex("dbo.UserVotes", new[] { "VoteId" });
            this.DropIndex("dbo.UserVotes", new[] { "UserId" });
            this.DropIndex("dbo.Votes", new[] { "ReplyId" });
            this.DropTable("dbo.UserVotes");
            this.DropTable("dbo.Votes");
        }
    }
}