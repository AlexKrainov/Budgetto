using Microsoft.AspNetCore.Mvc;
using MyProfile.Chat.Service;
using MyProfile.Entity.ModelView.Feedback;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class FeedbackController : Controller
    {
        private FeedbackService feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            this.feedbackService = feedbackService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveFeedback([FromBody] FeedbackCreateModelView feedback)
        {
            return Json(new { isOk = await feedbackService.Create(feedback) });
        }



    }
}
