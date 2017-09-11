namespace Blog.WebApplication.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using Data;
    using Models.BindingModels;

    public class WebContentController : ApiController
    {
        public bool GetApiMode()
        {
            var enviroment = HttpContext.Current.GetOwinContext().Environment;
            return enviroment.ContainsKey("host.AppMode") && enviroment["host.AppMode"].Equals("development");
        }

        public HttpResponseMessage GetReply(int replyId)
        {
            // http://localhost:49578/api/webcontent/reply?replyid=5
            var reply = BlogDbContext.Create().Replies.Find(replyId);
            if (reply == null)
            {
                ModelState.AddModelError(
                    "replyId",
                    new ArgumentException("Reply with this id is not found"));
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
            }

            var replyBM = new ReplyBindingModel
            {
                ReplyId = replyId,
                Text = reply.ReplayText,
                TopicId = reply.TopicId,
                ReplierId = reply.UserId,
                ReplyDateString = reply.ReplayDate.ToShortDateString()
            };

            var request = Request.CreateResponse(HttpStatusCode.OK);
            request.Content = new StringContent(
                System.Web.Helpers.Json.Encode(replyBM),
                Encoding.UTF8,
                "application/json");
            return request;
        }
    }
}