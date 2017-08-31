namespace Blog.Services
{
    using System.Drawing;
    using System.IO;
    using ImageProcessor;
    using ImageProcessor.Imaging;
    using ImageProcessor.Imaging.Formats;
    using Interfaces;

    public class ImageService : IImageService
    {
        private readonly ImageFactory imageFactory;

        public ImageService()
        {
            this.imageFactory = new ImageFactory();
        }

        public IImageService Load(string path)
        {
            this.imageFactory.Load(path);
            return this;
        }

        public IImageService Load(byte[] bytes)
        {
            this.imageFactory.Load(bytes);
            return this;
        }

        public IImageService GetRectangledImage()
        {
            var height = this.imageFactory.Image.Size.Height;
            var width = this.imageFactory.Image.Size.Width;
            if (height > width)
            {
                var diff = height - width;
                var halfDiff = diff / 2;
                this.imageFactory.Crop(new CropLayer(0, halfDiff, width, height - diff, CropMode.Pixels));
            }
            else if (height < width)
            {
                var diff = width - height;
                var halfDiff = diff / 2;
                this.imageFactory.Crop(new CropLayer(halfDiff, 0, width - diff, height, CropMode.Pixels));
            }

            return this;
        }

        public IImageService GetManualSizedRectangle(int size)
        {
            if (this.imageFactory.Image.Height > size)
            {
                this.imageFactory.Constrain(new Size(size, size));
            }

            return this;
        }

        public IImageService ToPng()
        {
            this.imageFactory.Format(new PngFormat());
            return this;
        }

        public IImageService ToJpeg()
        {
            this.imageFactory.Format(new JpegFormat());
            return this;
        }

        public IImageService To72DPI()
        {
            this.imageFactory.Resolution(72, 72);
            return this;
        }

        public void SaveTo(byte[] bytes, string path)
        {
            File.WriteAllBytes(this.GetCorrectFilename(path), bytes);
        }

        public void SaveTo(string path)
        {
            this.imageFactory.Save(this.GetCorrectFilename(path));
        }

        public byte[] ToByteArray()
        {
            var memoryStream = new MemoryStream();
            using (memoryStream)
            {
                this.imageFactory
                    .Image
                    .Save(memoryStream, this.imageFactory.CurrentImageFormat.ImageFormat);
            }

            return memoryStream.ToArray();
        }

        public string GetExtension()
        {
            return this.imageFactory.CurrentImageFormat.DefaultExtension;
        }

        private string GetCorrectFilename(string path)
        {
            var fileAndExtension = path.Split('.');
            var file = $"{fileAndExtension[0]}";
            fileAndExtension[1] = this.imageFactory.CurrentImageFormat.DefaultExtension;
            if (this.imageFactory.CurrentImageFormat.DefaultExtension == "jpeg")
            {
                fileAndExtension[1] = fileAndExtension[1].Replace("e", string.Empty);
            }

            return $"{file}.{fileAndExtension[1]}";
        }
    }
}