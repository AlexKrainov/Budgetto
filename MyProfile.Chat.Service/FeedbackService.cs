using Email.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Feedback;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Chat.Service
{
    using Chat = MyProfile.Entity.Model.Chat;
    public class FeedbackService
    {
        private UserEmailService userEmailService;
        private IBaseRepository repository;
        private FileWorkerService fileWorkerService;
        private UserLogService userLogService;

        public FeedbackService(UserEmailService userEmailService,
            IBaseRepository repository,
            FileWorkerService fileWorkerService,
            UserLogService userLogService )
        {
            this.userEmailService = userEmailService;
            this.repository = repository;
            this.fileWorkerService = fileWorkerService;
            this.userLogService = userLogService;
        }

        public async Task<bool> Create(FeedbackCreateModelView feedback)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            try
            {
                List<ResourceMessage> resourceMessages = new List<ResourceMessage>();
                foreach (var image in feedback.Images)
                {
                    if (!string.IsNullOrEmpty(image.ImageBase64))
                    {
                        var resource = new Resource
                        {
                            BodyBase64 = image.ImageBase64
                        };

                        try
                        {
                            fileWorkerService.CreateFileFromBase64(resource, ResourceFolder.Feedback);
                        }
                        catch (Exception ex)
                        {

                        }

                        resourceMessages.Add(new ResourceMessage
                        {
                            Resource = resource
                        });
                    }
                }

                var chatUser = new ChatUser
                {
                    IsChatOwner = true,
                    UserID = currentUser.ID,
                    DateAdded = now,
                };

                var chat = new Chat
                {
                    DateCreate = now,
                    DateEdit = now,
                    Title = feedback.Title ?? "Feedback",
                    Messages = new List<Message> { new Message
                    {
                        DateCreate = now,
                        DateEdit = now,
                        Text = feedback.Text,
                        ResourceMessages = resourceMessages,
                        ChatUser = chatUser
                    }
                    },
                    ChatUsers = new List<ChatUser> { chatUser },
                    Feedback = new Feedback
                    {
                        Priority = feedback.Priority,
                        Status = feedback.Status,
                        Topic = feedback.Topic.ToString(),
                        MoodID = feedback.MoodID
                    }
                };


                await repository.CreateAsync(chat, true);

                await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Feedback_Create);

                //userEmailSender.SendFeedback(chat);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
