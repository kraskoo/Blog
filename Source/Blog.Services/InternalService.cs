namespace Blog.Services
{
    using System.Linq;
    using Data;

    internal class InternalService
    {
        public static readonly DropboxService DropboxService;
        public static readonly ImageService ImageService;

        static InternalService()
        {
            var dropbox = BlogDbContext.Create()
                .ApiConnections
                .FirstOrDefault(api => api.ApiName == DropboxService.ApiName);
            DropboxService = new DropboxService(
                dropbox.Key,
                dropbox.Secrete,
                dropbox.Token);
            ImageService = new ImageService();
        }
    }
}