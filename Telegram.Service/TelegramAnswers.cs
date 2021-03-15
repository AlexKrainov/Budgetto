using MyProfile.Entity.ModelView.Notification;
using System.Globalization;
using System.Text;

namespace Telegram.Service
{
    public partial class TelegramService
    {

        private string StartAnswer()
        {
            return
@"Чтобы получать <b>уведомления</b>, отправьте сюда уникальный код.🙈

Код можно найти на сайте <a href='https://app.budgetto.org'>budgetto.org</a> в профиле пользователя (разделе <b>Подключения</b>)

Доступный функционал в этом режиме можно увидеть написав команду /help";
        }

        private string GoodAuthorizationAnswer(string name)
        {
            return
$@"Добрый день, <strong>{name}</strong>!👋
<strong>Ваш бот активирован!</strong>🎉🎉🎉

Здесь вы будете получать все настроенные уведомления в телеграм.👇
Спасибо что вы с нами 😉";
        }

        private string BadAuthorizationAnswer()
        {
            return
@"Извините, но введеный вами код <strong>не подошел</strong>! 🙁
Попробуйте еще раз.";
        }

        private string CommandNotRecognized()
        {
            return
@"Извините, но введеная вами команда <strong>не распознана</strong>! 🙁

Допустимые команды представлены при вызове /help.";
        }

        private string AlreadyConnected()
        {
            return
@"Вы уже <strong>подключены</strong> к телеграм-боту! 😉";
        }

        private string HelpAnswerSmall()
        {
            return
@"Доступные команды в этом режиме: 📃
/USD - курсы доллара на сегодня
/EUR - курсы евро на сегодня";
        }

        private string HelpAnswerFull()
        {
            return
@"Доступные команды: 📃
/accounts - Баланс всех счетов 

/stop - перестать получать уведомления

/USD - курсы доллара на сегодня
/EUR - курсы евро на сегодня";
        }
        ///limits - Лимиты
        ///goals - Цели
        ///reminders - Напоминания
        ///earning - Доходы
        ///spending - Расходы
        ///invest - Ивестировано

        private string GetAccountsAnswer(System.Collections.Generic.List<MyProfile.Entity.ModelView.Account.AccountViewModel> accounts, Model.TelegramUserModelView telegramUser)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("Ваши счета:");
            s.AppendLine("");

            foreach (var account in accounts)
            {
                NumberFormatInfo numberFormatInfo = new CultureInfo(account.Currency.specificCulture, false).NumberFormat;
                numberFormatInfo.CurrencyDecimalDigits = 0;

                var m = account.Balance.ToString("C", numberFormatInfo);

                if (account.AccountType == MyProfile.Entity.Model.AccountTypesEnum.Cash)
                {
                    s.AppendLine($"{m} - {account.Name}({account.AccountTypeName}) 💵");
                }
                else
                {
                    s.AppendLine($"{m} - {account.Name}({account.BankName}) 💳");
                }
            }

            return s.ToString();
            //@"Доступные команды в этом режиме: 📃
            ///USD - курсы доллара на сегодня 💳
            ///EUR - курсы евро на сегодня 💵 📈 💰 ";
        }

        private string Stop(string name)
        {
            return
$@"Теперь вы <b>не будете</b> получать уведомления от аккаунта <strong>{name}</strong>

Доступные команды в этом режиме: 📃
/USD - курсы доллара на сегодня
/EUR - курсы евро на сегодня";
        }






        #region Notifications

        private string GetLimitNotification(NotificationViewModel notification, string price)
        {
            return
$@"🔔 Уведомление по лимиту: <b>{notification.Name}</b>
Сумма достигла: <b>{price}</b>

Хорошего дня 😀";
        }
        private string GetReminderNotification(NotificationViewModel notification, string date)
        {
            return
$@"⏰ Напоминание: <b>{notification.Name}</b>
Дата: <b>{date}</b>

Хорошего дня 😀";
        }
        #endregion
    }
}
