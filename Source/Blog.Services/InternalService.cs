namespace Blog.Services
{
    using DataServices;

    internal class InternalService
    {
        public static readonly DropboxService DropboxService;
        public static readonly ImageService ImageService;

        static InternalService()
        {
            var dropbox = new ApiDataService().GetApiParametersByName(DropboxService.ApiName);
            DropboxService = new DropboxService(
                dropbox.Item1,
                dropbox.Item2,
                dropbox.Item3);
            ImageService = new ImageService();
        }
    }
}