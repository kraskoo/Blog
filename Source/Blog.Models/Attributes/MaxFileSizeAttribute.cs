namespace Blog.Models.Attributes
{
    using System.Web;
    using System.ComponentModel.DataAnnotations;

    public class MaxFileSizeAttribute : RequiredAttribute
    {
        public MaxFileSizeAttribute(int maxSize, int minSize = 0)
        {
            this.MaxSize = maxSize;
            this.MinSize = minSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"File size must be in range of [{this.MinSize}.. {(this.MaxSize / 1024 / (double) 1024):F2}] MB";
        }

        public override bool IsValid(object value)
        {

            if (!(value is HttpPostedFileBase httpPostFile))
            {
                return false;
            }

            return httpPostFile.ContentLength >= this.MinSize && httpPostFile.ContentLength <= this.MaxSize;
        }

        public int MaxSize { get; set; }

        public int MinSize { get; set; }
    }
}