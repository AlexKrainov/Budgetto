var BudgetTour;

$(function () {
    function setupTour(BudgetTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        BudgetTour.addStep({
            title: 'Выбор месяца',
            text: `<p>По умоланию отображается текущий месяц. 
Вы можете выбрать нужный месяц с помощью стрелок справа и слева от названия месяца.</p>`,
            attachTo: { element: '.budget-date', on: 'left' },
            buttons: [{
                action: BudgetTour.cancel,
                classes: backButtonClass,
                text: 'Выход'
            }, {
                action: BudgetTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        BudgetTour.addStep({
            title: 'Виджеты доходов и расходов',
            text: `В этой строке покзывается аналитика ваших доходов, 
расходов и инвестиций за выбранный месяц. Включить или отключить виджет можно, нажав на "шестеренки" справа.`,
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
            text: `Для работы с таблицей и правильного отображения аналитики в виджетах выберите шаблон из ранее созданных. 
Если вы создавали только один шаблон, он будет отображаться автоматически.`,
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
            title: 'Ваши финансы',
            text: `В таблице отображается выбранный вами в шаге 4 шаблон. Колонки - созданные вами ранее категории.`,
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
            title: 'Создание категорий',
            text: `Здесь можно создвать категории трат для их удобного отображения в таблице финансов (например, еда = рестораны + продукты) `,
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
            text: `Здесь можно организовать категории в шаблоны для удобства ипользования таблицы "финансы" (например, шаблон "ремонт" или "подготовка к Новому году")`,
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
            title: 'Настройки',
            text: `Возможность активации виджетов на главной странице программы`,
            attachTo: { element: '.theme-settings-open-btn2 i', on: 'left' },
            buttons: [{
                action: BudgetTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: BudgetTour.next,
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

    $("#hint-show-page").click(function () {
        setupTour(BudgetTour).start();
    });

});