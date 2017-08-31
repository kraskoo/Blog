namespace Blog.Services.DataServices
{
    using Data;

    public abstract class BaseDataService
    {
        protected BlogDbContext GetDbContext => BlogDbContext.Create();
    }
}