var BudgetTour;

$(function () {

    if (window.layoutHelpers.isSmallScreen()) {
        return;
    }
    function setupTour(BudgetTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        if (UserInfo.UserSettings.IsShowFirstEnterHint) {
            BudgetTour.addStep({
                title: 'Здравствуйте!',
                text: `<p>Добро пожаловать в Budgetto.</p> 
<p>Так как вы впервые используете программу, предлагаем вам пройти ознакомительный тур. Он состоит из 8 шагов - это быстро! </p>
<p>Нажмите кнопку "Далее", чтобы начать работу.</p>`,
                //attachTo: { element: '.navbar-user', on: 'left' },
                buttons: [{
                    action: NotShowEnterHint,
                    classes: 'btn btn-sm btn-secondary float-left',
                    text: 'Больше не показывать'
                }, {
                    action: BudgetTour.cancel,
                    classes: backButtonClass,
                    text: 'Закрыть'
                }, {
                    action: BudgetTour.next,
                    classes: nextButtonClass,
                    text: 'Далее'
                }]
            });
        }

        let buttons = [];
        if (UserInfo.UserSettings.IsShowFirstEnterHint == false) {
            buttons.push({
                action: BudgetTour.cancel,
                classes: backButtonClass,
                text: 'Закрыть'
            });
        }
        buttons.push({
            action: BudgetTour.next,
            classes: nextButtonClass,
            text: 'Далее'
        });

        BudgetTour.addStep({
            title: 'Выбор месяца',
            text: `<p>По умоланию отображается текущий месяц. 
Вы можете выбрать нужный месяц с помощью стрелок справа и слева от названия месяца.</p>`,
            attachTo: { element: '.budget-date', on: 'left' },
            buttons: buttons
        });
        BudgetTour.addStep({
            title: 'Виджеты доходов, расходов и инвестиций',
            text: `В этих виджетах отображаются все ваши внесенные данные за выбранный месяц.`,
            attachTo: { element: '.total-view', on: 'bottom' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Выбор шаблона',
            text: `Выбранный шаблон отображется в таблице. Вы можете создавать несколько шаблонов.`,
            attachTo: { element: '#templates', on: 'right' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Главная таблица',
            text: `В таблице отображаются все внесенные вами данные за выбранный месяц. Колонки формируются на основании шаблона по вашим категориям.`,
            //text: `В таблице отображаннннннннтся ваши деньги в привязке ко дню за календарный месяц, колонки строятся по вашим категориям. Категории можно группировать.`,
            attachTo: { element: '.card-datatable', on: 'top' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Категории',
            text: `Здесь можно создвать, редактировать, удалять и группировать категории по папкам.`,
            attachTo: { element: 'a[href="/Section/Edit"]', on: 'right' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Шаблоны',
            text: `На этой странице вы можете создавать, редактировать и удалять шаблоны для работы на страницах "Финансы на месяц" и "Финансы на год".`,
            attachTo: { element: 'a[href="/Template/List"]', on: 'right' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Добавление записей.',
            text: `Учет финансов начинается с добавления записей в таблицу. Чтобы добавить запись, нажмите "+"`,
            attachTo: { element: '#addRecord', on: 'bottom' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Настройки страницы',
            text: `Возможность показывать и скрывать виджеты на этой странице.`,
            attachTo: { element: '.theme-settings-open-btn2 i', on: 'left' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Окончание тура',
            text: `Здорово! Вы ознакомились с базовыми функциями программы. Посмотреть этот тур и другие вы можете, нажав на сивол "?" в правом верхнем углу экрана.`,
            //attachTo: { element: '.theme-settings-open-btn2 i', on: 'left' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: UserInfo.UserSettings.IsShowFirstEnterHint ? NotShowEnterHint : BudgetTour.cancel,
                classes: nextButtonClass,
                text: 'Завершить'
            }]
        });

        return BudgetTour;
    }

    BudgetTour = new Shepherd.Tour({
        includeStyles: false,

        defaultStepOptions: {
            scrollTo: false,
            cancelIcon: {
                enabled: true
            }
        },
        useModalOverlay: true
    });

    if (UserInfo.UserSettings.IsShowFirstEnterHint) {
        setupTour(BudgetTour).start();
    }

    $("#hint-show-page").click(function () {
        setupTour(BudgetTour).start();
    });

});

function NotShowEnterHint() {
    UserInfo.UserSettings.IsShowFirstEnterHint = false;
    BudgetTour.cancel();
    return $.ajax({
        type: "GET",
        url: "/UserSettings/NotShowEnterHint",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            return response;
        },
        error: function (xhr, status, error) {
        }
    });
}