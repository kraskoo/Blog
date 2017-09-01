namespace Blog.WebApplication.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Models;
    using Models.BindingModels;
    using Services.DataServices;

    public class ReplyController : Controller
    {
        private readonly ReplyDataService dataService;

        public ReplyController()
        {
            this.dataService = new ReplyDataService();
        }

        [HttpPost]
        public async Task<ActionResult> SendReply(ReplyBindingModel reply)
        {
            var returnUrl = this.Request.UrlReferrer?.AbsolutePath;
            if (!this.ModelState.IsValid)
            {
                return await Task.FromResult(this.Redirect(returnUrl));
            }

            await this.dataService.CreateReply(reply);
            return await Task.FromResult(this.Redirect(returnUrl));
        }

        public async Task<ActionResult> DeleteReply(int replyId)
        {
            var returnUrl = this.Request.UrlReferrer?.AbsolutePath;
            var isExists = await this.dataService.ReplyExists(replyId);
            if (!isExists)
            {
                this.ModelState.AddModelError("", @"Reply doesn't exists.");
                return await Task.FromResult(this.Redirect(returnUrl));
            }

            await this.dataService.DeleteReply(replyId);
            return await Task.FromResult(this.Redirect(returnUrl));
        }

        [HttpGet]
        public async Task<ActionResult> EditInModal(int replyId)
        {
            var reply = await this.dataService.GetReplyById(replyId);
            return await Task.FromResult(this.View(reply));
        }

        [HttpPost]
        public ActionResult EditInModal(Reply reply)
        {
            var returnUrl = this.Request.UrlReferrer?.AbsolutePath;
            if (!this.ModelState.IsValid)
            {
                return this.View(reply);
            }

            this.dataService.UpdateModelByModal(reply);
            return this.Redirect(returnUrl);
        }

        public async Task<ActionResult> Edit(int replyId)
        {
            var requestedReply = await this.dataService.GetReplyById(replyId);
            if (requestedReply == null)
            {
                return await Task.FromResult(new HttpStatusCodeResult(HttpStatusCode.NotFound));
            }

            if (requestedReply.User.UserName != this.User.Identity.Name)
            {
                return await Task.FromResult(new HttpStatusCodeResult(HttpStatusCode.Forbidden));
            }

            var newBindingModel = new ReplyBindingModel
            {
                ReplyId = requestedReply.Id,
                TopicId = requestedReply.TopicId,
                Text = requestedReply.ReplayText,
                ReplierId = requestedReply.UserId
            };
            return await Task.FromResult(this.View(newBindingModel));
        }

        [HttpPost]
        public ActionResult Edit(ReplyBindingModel rbm)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(rbm);
            }

            this.dataService.UpdateModel(rbm);
            return this.RedirectToAction("Index", "Home");
        }
    }
}