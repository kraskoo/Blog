namespace Blog.Services.Interfaces
{
    public interface IImageService
    {
        IImageService Load(string path);

        IImageService Load(byte[] bytes);

        IImageService GetRectangledImage();

        IImageService GetManualSizedRectangle(int size);

        IImageService ToPng();

        IImageService ToJpeg();

        IImageService To72DPI();

        void SaveTo(byte[] bytes, string path);

        void SaveTo(string path);

        byte[] ToByteArray();

        string GetExtension();
    }
}