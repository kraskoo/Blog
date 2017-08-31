namespace Blog.Data
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    public class BlogDbContext : IdentityDbContext<User>
    {
        private const string DefaultContextName = nameof(BlogDbContext);

        public BlogDbContext() : base(DefaultContextName, false)
        {
        }

        public IDbSet<Topic> Topics { get; set; }

        public IDbSet<Reply> Replies { get; set; }

        public IDbSet<ApiConnection> ApiConnections { get; set; }

        public static BlogDbContext Create()
        {
            return new BlogDbContext();
        }
    }
}