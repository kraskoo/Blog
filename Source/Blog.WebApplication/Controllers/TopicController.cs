namespace Blog.WebApplication.Controllers
{
    using System.Web.Mvc;
    using Models.BindingModels;
    using Services.DataServices;

    public class TopicController : Controller
    {
        private readonly TopicDataService dataService;

        public TopicController()
        {
            this.dataService = new TopicDataService();
        }

        [Route("Topic/{id:int}")]
        public ActionResult GetById(int id)
        {
            return this.View(this.dataService.GetById(id));
        }

        [Route("Topic")]
        public ActionResult Index(string category = null)
        {
            return this.View(this.dataService.GetIndexTopics(category));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            return this.View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(TopicBindingModel topic)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(topic);
            }

            this.dataService.CreateNewTopic(topic, this.User);
            return this.RedirectToAction("Index", "Topic");
        }
    }
}