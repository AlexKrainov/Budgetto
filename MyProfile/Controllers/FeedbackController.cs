﻿using Microsoft.AspNetCore.Mvc;
using MyProfile.Chat.Service;
using MyProfile.Entity.ModelView.Feedback;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class FeedbackController : Controller
    {
        private UserLogService userLogService;
        private FeedbackService feedbackService;

        public FeedbackController(FeedbackService feedbackService, UserLogService userLogService)
        {
            this.userLogService = userLogService;
            this.feedbackService = feedbackService;
        }

        public async Task<IActionResult> Create()
        {
            await userLogService.CreateUserLog(UserInfo.Current.UserSessionID, UserLogActionType.Feedback_Page);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveFeedback([FromBody] FeedbackCreateModelView feedback)
        {
            return Json(new { isOk = await feedbackService.Create(feedback) });
        }



    }
}
