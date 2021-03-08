using MyProfile.Entity.ModelView.Notification;

namespace Telegram.Service
{
    public partial class TelegramService
    {

        private string StartAnswer()
        {
            return
@"Чтобы получать <b>уведомления</b> отправте сюда уникальный код.🙈

Код можно найти в профиле пользователя на сайте <a href='https://app.budgetto.org'>budgetto.org</a> в разделе <b>Подключения</b>

Доступный функционал в этом режиме можно увидеть написав команду /help";
        }

        private string GoodAuthorizationAnswer(string name)
        {
            return
$@"Добрый день, <strong>{name}</strong>!👋
<strong>Ваш бот активирован!</strong>🎉🎉🎉

Вы начнете получать ваши настроенные уведомления на телеграм прямо здесь.👇
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
/limits - Лимиты
/goals - Цели
/reminders - Напоминания
/earning - Доходы
/spending - Расходы
/invest - Ивестировано

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

        #endregion
    }
}
