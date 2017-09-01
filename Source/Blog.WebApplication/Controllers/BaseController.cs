namespace Blog.WebApplication.Controllers
{
    using System.Web.Http;
    using System.Web.Http.ModelBinding;

    public class BaseController : ApiController
    {
        public JQueryMvcFormUrlEncodedFormatter GetContent()
        {
            return new JQueryMvcFormUrlEncodedFormatter(this.Configuration);
        }
    }
}