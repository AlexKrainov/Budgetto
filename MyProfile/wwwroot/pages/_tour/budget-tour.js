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
            title: 'Виджеты доходов, расходов и инвестиций',
            text: `В этих виджетах учитываются все ваши внесенные данные за выбранный месяц.`,
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
            text: `На этой странице вы можете создавать, редактировать и удалять шаблоны для работы на страницах "Бюджет на месяц" и "Бюджет на год".`,
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