namespace Blog.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;

    public sealed class Configuration : DbMigrationsConfiguration<BlogDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BlogDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.Add(new IdentityRole("Admin"));
                context.Roles.Add(new IdentityRole("Regular"));
                context.SaveChanges();
            }
        }
    }
}