namespace Blog.WebApplication.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Data;
    using Models.BindingModels;
    using Services;
    using Services.AccountServices;
    using Services.DataServices;

    public class WebContentController : ApiController
    {
        private AccountProfileService profileService;

        public WebContentController()
        {
            this.profileService = new AccountProfileService();
        }

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

        public async Task<HttpResponseMessage> GetComments(int topicId)
        {
            var replyDataService = new ReplyDataService();
            var topicReplies = await replyDataService.GetRepliesByTopicId(topicId);
            var request = Request.CreateResponse(HttpStatusCode.OK);
            request.Content = new StringContent(
                System.Web.Helpers.Json.Encode(topicReplies),
                Encoding.UTF8,
                "application/json");
            return request;
        }

        public async Task<HttpResponseMessage> GetIsCurrentUserWatchTopicPage(string userId, int replyId, int key)
        {
            var returnObject = new
            {
                IsSameUser = HttpContext.Current.User.Identity.GetUserId() == userId,
                ReplyId = replyId,
                Key = key
            };
            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    System.Web.Helpers.Json.Encode(returnObject),
                    Encoding.UTF8,
                    "application/json")
            });
        }

        [OutputCache(Duration = 120)]
        public async Task<HttpResponseMessage> GetUserProfileImage(string userId)
        {
            var currentObject = new
            {
                UserId = userId,
                Img = this.profileService.GetUserProfileImage(HttpContext.Current.GetOwinContext().GetUserManager<UserManager>().Users.FirstOrDefault(u => u.Id == userId))
            };
            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    System.Web.Helpers.Json.Encode(currentObject),
                    Encoding.UTF8,
                    "application/json")
            });
        }
    }
}