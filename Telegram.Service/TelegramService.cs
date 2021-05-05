using Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.ModelView.User;
using MyProfile.Entity.Repository;
using MyProfile.Goal.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Service.Model;
using ChatType = MyProfile.Entity.Model.ChatType;

namespace Telegram.Service
{
    public partial class TelegramService : IHostedService, IDisposable
    {
        private BaseRepository repository;
        private IMemoryCache cache;
        private IServiceScopeFactory scopeFactory;
        private TelegramBotUser telegramBotUser;
        private TelegramBotClient botClient;
        private DateTime now;

        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts =
                                                       new CancellationTokenSource();

        public TelegramService(BaseRepository repository,
            IMemoryCache cache,
            IServiceScopeFactory scopeFactory)
        {
            this.repository = repository;
            this.cache = cache;
            this.scopeFactory = scopeFactory;

            telegramBotUser = GetTelegramBotUser();
            botClient = new TelegramBotClient(PublishSettings.TelegramApi);
        }

        private TelegramBotUser GetTelegramBotUser()
        {
            TelegramBotUser telegramBotUser;

            if (cache.TryGetValue(typeof(TelegramBotUser).Name, out telegramBotUser) == false)
            {
                telegramBotUser = repository.GetAll<User>(x => x.UserTypeID == (int)UserTypeEnum.TelegramBot && x.IsDeleted == false)
                    .Select(x => new TelegramBotUser
                    {
                        ID = x.ID
                    })
                .FirstOrDefault();

                cache.Set(typeof(TelegramBotUser).Name, telegramBotUser, DateTime.Now.AddDays(15));
            }
            return telegramBotUser;
        }

        public void Start()
        {
            //var me = botClient.GetMeAsync().Result;

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnMessageEdited += Bot_OnMessage;

            botClient.StartReceiving();
        }


        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var text = e.Message.Text.Trim();

            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    #region initialization
                    repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                    cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

                    telegramBotUser = GetTelegramBotUser();
                    botClient = new TelegramBotClient(PublishSettings.TelegramApi);

                    now = DateTime.Now.ToUniversalTime();
                    string answer = string.Empty;

                    var telegramUser = await GetOrCreateTelegramAccount(e.Message);
                    #endregion

                    #region Authorization telegram account
                    if (text.StartsWith("telegram_"))
                    {
                        if (telegramUser.StatusID != (int)TelegramAccountStatusEnum.Connected)//telegramUser.UserID == null)
                        {
                            var userConnect = await repository.GetAll<UserConnect>(x => x.TelegramLogin == text)
                                .FirstOrDefaultAsync();

                            if (userConnect != null)
                            {
                                var dbTelegramAccount = await repository.GetAll<TelegramAccount>(x => x.ID == telegramUser.TelegramAccountID)
                                    .FirstOrDefaultAsync();
                                dbTelegramAccount.UserID = telegramUser.UserID = userConnect.UserID;
                                dbTelegramAccount.StatusID = telegramUser.StatusID = (int)TelegramAccountStatusEnum.Connected;
                                dbTelegramAccount.DateEdit = now;
                                telegramUser.UserName = userConnect.User.Name;

                                repository.Create(new Notification
                                {
                                    IsReady = true,
                                    IsSite = true,
                                    LastChangeDateTime = now,
                                    IsReadyDateTime = DateTime.Now,
                                    NotificationTypeID = (int)NotificationType.Telegram,
                                    TelegramAccountID = dbTelegramAccount.ID,
                                    UserID = userConnect.UserID,
                                });

                                await repository.SaveAsync();

                                answer = GoodAuthorizationAnswer(telegramUser.UserName);
                            }
                            else
                            {
                                answer = BadAuthorizationAnswer();
                            }
                        }
                        else
                        {
                            //Вы подключили еще один аккаунт
                            answer = AlreadyConnected();
                        }

                        await SaveMessages(telegramUser, e.Message, answer);
                        await botClient.SendTextMessageAsync(
                             chatId: e.Message.Chat,
                             text: answer,
                             parseMode: ParseMode.Html
                           );
                        return;
                    }
                    #endregion

                    switch (text)
                    {
                        case "/help":
                            if (telegramUser.StatusID == (int)TelegramAccountStatusEnum.Connected)
                            {
                                answer = HelpAnswerFull();
                            }
                            else
                            {
                                answer = HelpAnswerSmall();
                            }
                            break;
                        case "/USD":
                        case "/EUR":
                            var currencyService = scope.ServiceProvider.GetRequiredService<CurrencyService>();
                            var z = await currencyService.GetRateByCodeAsync(now, text.Replace("/", ""));
                            NumberFormatInfo numberFormatInfo = new CultureInfo("ru-RU", false).NumberFormat;
                            numberFormatInfo.CurrencyDecimalDigits = 2;

                            answer = z.Rate.ToString("C", numberFormatInfo);
                            break;
                        case "/stop":
                            answer = Stop(telegramUser.UserName);

                            var account = await repository.GetAll<TelegramAccount>(x => x.ID == telegramUser.TelegramAccountID
                                    && x.StatusID != (int)TelegramAccountStatusEnum.Locked)
                                .FirstOrDefaultAsync();
                            account.StatusID = (int)TelegramAccountStatusEnum.Locked;
                            account.DateEdit = now;
                            await repository.SaveAsync();

                            break;
                        case "/accounts":
                            if (telegramUser.StatusID == (int)TelegramAccountStatusEnum.Connected
                                || telegramUser.StatusID == (int)TelegramAccountStatusEnum.InPause)
                            {
                                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                                var accounts = accountService.GetAccountsWithoutParant(telegramUser.UserID ?? Guid.NewGuid());

                                answer = GetAccountsAnswer(accounts, telegramUser);
                            }
                            else
                            {
                                answer = CommandNotRecognized();
                            }
                            break;
                        //case "/goals":
                        //    if (telegramUser.StatusID == (int)TelegramAccountStatusEnum.Connected
                        //        || telegramUser.StatusID == (int)TelegramAccountStatusEnum.InPause)
                        //    {
                        //        var goalService = scope.ServiceProvider.GetRequiredService<GoalService>();

                        //        var goals = goalService.get

                        //        answer = GetAccountsAnswer(accounts, telegramUser);
                        //    }
                        //    else
                        //    {
                        //        answer = CommandNotRecognized();
                        //    }
                        //    break;
                        case "/start":
                            if (telegramUser.StatusID == (int)TelegramAccountStatusEnum.Connected)
                            {
                                answer = HelpAnswerFull();
                            }
                            else if (telegramUser.StatusID == (int)TelegramAccountStatusEnum.InPause)
                            {
                                answer = HelpAnswerSmall();
                            }
                            else
                            {
                                answer = StartAnswer();
                            }
                            break;
                        default:
                            answer = CommandNotRecognized();
                            break;
                    }

                    await SaveMessages(telegramUser, e.Message, answer);

                    await botClient.SendTextMessageAsync(
                         chatId: e.Message.Chat,
                         text: answer,
                         parseMode: ParseMode.Html
                       );
                }
                catch (Exception ex)
                {
                    // ToDo: send email and write to the ErrorLogs
                    await repository.CreateAsync(new ErrorLog
                    {
                        ErrorText = ex.Message,
                        CurrentDate = DateTime.Now.ToUniversalTime(),
                        Where = "TelegramService.Bot_OnMessage",
                        Comment = $"{{TelegramID:{e.Message.From.Id}, Text:{e.Message.Text}, FirstName:{e.Message.From.FirstName} }}"
                    }, true);
                }
            }

        }

        public async Task SetNewStatus(int accountID, TelegramAccountStatusEnum newStatusID)
        {
            var dbTelegramAccount = await repository.GetAll<TelegramAccount>(x => x.ID == accountID)
                                .FirstOrDefaultAsync();
            dbTelegramAccount.StatusID = (int)newStatusID;
            dbTelegramAccount.DateEdit = now;
            
            await repository.SaveAsync();
            return;
        }

        private async Task SaveMessages(TelegramUserModelView telegramUser, Bot.Types.Message message, string answer)
        {
            await repository.CreateRangeAsync(new List<Message> {
                        new Message
                        {
                            ChatID = telegramUser.ChatID,
                            ChatUserID = telegramUser.ChatUserID,
                            DateCreate = now,
                            Text = message.Text
                        },
                        new Message
                        {
                            ChatID = telegramUser.ChatID,
                            ChatUserID = telegramUser.ChatTelegramBotUserID,
                            Text = answer,
                            DateCreate = now.AddSeconds(1),
                        }
                    }, true);
        }

        private async Task SaveMessageFromTelegramBot(long chatID, string answer)
        {
            long telegramBotChatUserID = await repository.GetAll<ChatUser>(x => x.ChatID == chatID && x.UserID == telegramBotUser.ID)
                .Select(x => x.ID)
                .FirstOrDefaultAsync();

            if (telegramBotChatUserID != 0)
            {
                await repository.CreateRangeAsync(new List<Message> {
                        new Message
                        {
                            ChatID = chatID,
                            ChatUserID = telegramBotChatUserID,
                            DateCreate = DateTime.Now.ToUniversalTime(),
                            Text = answer
                        }
                    }, true);
            }
        }

        private async Task<TelegramUserModelView> GetOrCreateTelegramAccount(Bot.Types.Message message)
        {
            if (await repository.AnyAsync<TelegramAccount>(x => x.TelegramID == message.From.Id) == false)
            {
                //create
                await repository.CreateAsync(new TelegramAccount
                {
                    DateCreate = now,
                    DateEdit = now,
                    LastDateConnect = now,
                    Description = message.Chat.Description,
                    Title = message.Chat.Title,
                    FirstName = message.From.FirstName,
                    LanguageCode = message.From.LanguageCode,
                    TelegramID = message.From.Id,
                    LastName = message.From.LastName,
                    StatusID = (int)TelegramAccountStatusEnum.New,
                    Username = message.From.Username,
                }, true);

                var newTelegramAccount = await repository.GetAll<TelegramAccount>(x => x.TelegramID == message.From.Id)
                    .Select(x => new
                    {
                        ID = x.ID
                    })
                    .FirstOrDefaultAsync();

                await repository.CreateAsync(new Chat
                {
                    ChatType = (int)ChatType.UserToTelegramBot,
                    DateCreate = now,
                    DateEdit = now,
                    Title = "Пользователь с Budgetto_bot (Telegram)",
                    ChatUsers = new List<ChatUser> {
                        new ChatUser
                        {
                            DateAdded = now,
                            IsChatOwner = true,
                            TelegramAccountID = newTelegramAccount.ID,
                        },
                        new ChatUser {
                            DateAdded = now,
                            UserID = telegramBotUser.ID
                        }
                    },
                }, true);

            }

            var dbuser = await repository.GetAll<TelegramAccount>(x =>
                    x.TelegramID == message.From.Id)
                .FirstOrDefaultAsync();
            dbuser.LastDateConnect = now;
            await repository.SaveAsync();

            return new TelegramUserModelView
            {
                TelegramAccountID = dbuser.ID,
                StatusID = dbuser.StatusID,
                TelegramLogin = dbuser.UserConnect != null ? dbuser.UserConnect.TelegramLogin : null,
                UserID = dbuser.UserID,
                ChatID = dbuser.ChatUsers.FirstOrDefault().ChatID,
                ChatUserID = dbuser.ChatUsers.FirstOrDefault().ID,
                ChatTelegramBotUserID = dbuser.ChatUsers.FirstOrDefault().Chat.ChatUsers.FirstOrDefault(x => x.UserID == telegramBotUser.ID).ID,
                UserName = dbuser.UserID != null ? dbuser.UserConnect.User.Name : null,
            };
        }


        public async Task<bool> SendNotification(NotificationViewModel notification)
        {
            try
            {
                string answer = string.Empty;
                bool isSent = false;

                switch (notification.NotificationTypeID)
                {
                    case (int)NotificationType.Limit:
                        NumberFormatInfo numberFormatInfo = new CultureInfo("ru-RU", false).NumberFormat;
                        numberFormatInfo.CurrencyDecimalDigits = 0;

                        answer = GetLimitNotification(notification, notification.Total?.ToString("C", numberFormatInfo));
                        break;
                    case (int)NotificationType.Reminder:
                        answer = GetReminderNotification(notification, notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes).ToString("dd.MM.yyyy HH:mm"));
                        break;
                    default:
                        break;
                }

                foreach (var telegramAccount in notification.TelegramAccounts)
                {
                    if (telegramAccount.StatusID == (int)TelegramAccountStatusEnum.Connected)
                    {
                        await botClient.SendTextMessageAsync(
                                           chatId: new Bot.Types.ChatId(long.Parse(telegramAccount.TelegramID)),
                                           text: answer,
                                           parseMode: ParseMode.Html
                                           );

                        await SaveMessageFromTelegramBot(telegramAccount.ChatID, answer);
                    }
                    isSent = true;
                }
                return isSent;
            }
            catch (Exception ex)
            {
                await repository.CreateAsync(new ErrorLog
                {
                    ErrorText = ex.Message,
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Where = "TelegramService.SendNotification",
                    Comment = "NotificationOnTelegramTask"
                }, true);
            }
            return false;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            // _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                              cancellationToken));
            }

        }

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
