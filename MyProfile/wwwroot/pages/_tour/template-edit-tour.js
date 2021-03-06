var TemplateTour;

$(function () {
    if (window.layoutHelpers.isSmallScreen()) {
        return;
    }
    function setupTour(TemplateTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        TemplateTour.addStep({
            title: 'Выбор типа шаблона',
            text: 'Нажмите на стрелку вниз',
            attachTo: { element: '.template-types button ', on: 'right' },
            buttons: []
        });
        TemplateTour.addStep({
            title: 'Выбор типа шаблона',
            text: 'Выберите страницу для отображения шаблона, например "Финансы на месяц"',
            attachTo: { element: '.template-types .dropdown-menu', on: 'right' },
            buttons: []
        });
        TemplateTour.addStep({
            title: 'Название',
            text: 'Введите название шаблона',
            attachTo: { element: '#template-name', on: 'right' },
            buttons: [{
                action: TemplateTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        TemplateTour.addStep({
            title: 'Добавление первой колонки',
            text: 'Нажмите "Добавить колонку", чтобы выбрать тип колонки',
            attachTo: { element: '#add-column', on: 'top' },
            buttons: []
        });
        TemplateTour.addStep({
            title: 'Выбор типа колонки',
            text: 'Для начала выберите "Дни"',
            attachTo: { element: '#template-days', on: 'top' },
            buttons: []
        });
        TemplateTour.addStep({
            title: 'Первая колонка',
            text: 'В первой колонке будут отображаться дни месяца. В "настройках колонки" можно задать формат даты.',
            attachTo: { element: '.lists .card', on: 'top' },
            buttons: [{
                action: function () {
                    TemplateTour.next();

                    $("#add-column").click(function () {
                        setTimeout(TemplateTour.next, 700);
                        $("#add-column").off();

                        $("#template-sections").click(function () {
                            setTimeout(TemplateTour.next, 700);
                            $("#template-sections").off();

                            //$(".cards-big .card-section").click(function () {
                            //    setTimeout(TemplateTour.next, 500);
                            //    $(".cards-big .card-section").off();

                            //$(".add-section").click(function () {
                            //    setTimeout(TemplateTour.next, 800);
                            //    $(".add-section").off();

                            //    $(".cards-big .card-section").click(function () {
                            //        setTimeout(TemplateTour.next, 500);
                            //        $(".cards-big .card-section").off();

                            $(".save-template").click(function () {
                                setTimeout(TemplateTour.next, 1000);
                                $(".save-template").off();
                            });
                            //    });

                            //});
                            // });
                        });
                    });
                },
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        TemplateTour.addStep({
            title: 'Второй колонки',
            text: 'Добавление второй колонки',
            attachTo: { element: '#add-column', on: 'top' },
            buttons: []
        });
        TemplateTour.addStep({
            title: 'Выбор типа колонки',
            text: 'Выберите тип колонки "Категории".',
            attachTo: { element: '#template-sections', on: 'bottom' },
            buttons: []
        });

        TemplateTour.addStep({
            title: 'Выбор категорий',
            text: 'Выберите категории, которые будут отображаться во второй колонке в таблице, и нажмите "Далее".',
            attachTo: { element: '.cards-big', on: 'top' },
            buttons: [{
                action: function () {
                    TemplateTour.next()
                    $("#close-sections").click();
                },
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        TemplateTour.addStep({
            title: 'Вторая колонка',
            text: 'Вторая колонка состоит из категорий, по умолчанию они будут сложены в итоговую сумму в таблицу для конкретного дня. Здесь вы можете дать название колонке, добавить/удалить категории и в настройках выбрать дополнительные опции.',
            attachTo: { element: '.lists .list:nth-child(2) .card', on: 'top' },
            buttons: [{
                action: TemplateTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
         TemplateTour.addStep({
             title: 'Завершение тура',
            text: 'Все остальные колонки с данными формируются таким же способом.',
            buttons: [{
                action: TemplateTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        //TemplateTour.addStep({
        //    title: 'Добавление категорий',
        //    text: 'Вы можете добавить сколько угодно категорий в колонку, по умолчанию все категории в колонке будут складываться.',
        //    attachTo: { element: '.add-section', on: 'top' },
        //    buttons: []
        //});
        //TemplateTour.addStep({
        //    title: 'Выберите категорию',
        //    text: 'Выберите дополнительную категорию.',
        //    attachTo: { element: '.cards-big', on: 'left' },
        //    buttons: []
        //});
        //TemplateTour.addStep({
        //    title: 'Колонка с категориями',
        //    text: 'В этой колонке у вас теперь 2 категории, по умолчанию они будут сложены в итоговую сумму в таблицу для конкретного дня.',
        //    attachTo: { element: '.lists .list:nth-child(2)', on: 'top' },
        //    buttons: [{
        //        action: TemplateTour.next,
        //        classes: nextButtonClass,
        //        text: 'Далее'
        //    }]
        //});
        TemplateTour.addStep({
            title: 'Сохранение шаблона',
            text: 'После сохранения шаблона вы сможете увидеть его на странице "Бюджета на месяц", выбрав его из списка.',
            attachTo: { element: '.save-template', on: 'bottom' },
            buttons: [{
                action: TemplateTour.cancel,
                classes: nextButtonClass,
                text: 'Завершить'
            }]
        });

        return TemplateTour;
    }

    TemplateTour = new Shepherd.Tour({
        includeStyles: false,

        defaultStepOptions: {
            scrollTo: false,
            cancelIcon: {
                enabled: true
            }
        },
        useModalOverlay: true
    });

    TemplateTour.on('cancel', () => {
        $(".template-types button").off();
        $(".template-types .dropdown-item").off();
        $("#add-column").off();
        $("#template-days").off();

        $("#add-column").off();
        $("#template-sections").off();
        $(".cards-big .card-section").off();
        $(".add-section").off();
        $(".cards-big .card-section").off();
        $(".save-template").off();
    });

    $("#hint-show-page").click(function () {
        setupTour(TemplateTour).start();

        $(".template-types button").click(function () {
            setTimeout(TemplateTour.next, 200);
            $(".template-types button").off();

            $(".template-types .dropdown-item").click(function () {
                setTimeout(TemplateTour.next, 200);
                $(".template-types .dropdown-item").off();

                $("#add-column").click(function () {
                    setTimeout(TemplateTour.next, 600);
                    $("#add-column").off();

                    $("#template-days").click(function () {
                        setTimeout(TemplateTour.next, 600);
                        $("#template-days").off();

                    });
                });
            });
        });
    });
});