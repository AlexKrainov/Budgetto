var AddSectionTour;

$(function () {
    if (window.layoutHelpers.isSmallScreen()) {
        return;
    }

    function setupTour(AddSectionTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        AddSectionTour.addStep({
            title: 'Выбор группы категорий',
            text: 'Для создании категории нажмите "показать категории".',
            attachTo: { element: '.show-sections', on: 'left' },
            buttons: [{
                action: function () {
                    AddSectionTour.cancel();
                    $($(".show-sections")[0]).off();
                },
                classes: backButtonClass,
                text: 'Завершить'
            }]
        });
        AddSectionTour.addStep({
            title: 'Кнопка "Создать категорию"',
            text: 'Нажмите кнопку "Создать категорию"',
            attachTo: { element: '#add-section', on: 'bottom' },
            buttons: []
        });
        AddSectionTour.addStep({
            title: 'Тип категории',
            text: 'Для начала выберите тип категории. Это могут быть расходы, доходы или инвестиции.',
            attachTo: { element: '.section-types', on: 'bottom' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        AddSectionTour.addStep({
            title: 'Название',
            text: 'Введите название категории.',
            attachTo: { element: '#section-name', on: 'bottom' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        AddSectionTour.addStep({
            title: 'Цвет',
            text: 'Нажмите на стрелку справа для выбора цвета категории.',
            attachTo: { element: '.choose-color', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        AddSectionTour.addStep({
            title: 'Иконка',
            text: 'Выберите иконку.',
            attachTo: { element: '.choose-icons', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        AddSectionTour.addStep({
            title: 'Сохранение',
            text: 'Нажмите кнопку сохранить',
            attachTo: { element: '#save-sections', on: 'top' },
            buttons: [{
                action: AddSectionTour.cancel,
                classes: nextButtonClass,
                text: 'Завершить'
            }]
        });

        return AddSectionTour;
    }

    AddSectionTour = new Shepherd.Tour({
        includeStyles: false,

        defaultStepOptions: {
            scrollTo: false,
            cancelIcon: {
                enabled: true
            }
        },
        useModalOverlay: true
    });

    AddSectionTour.on('cancel', () => {
        $($(".show-sections")[0]).off();
        $("#add-section").off();
    });

    $("#hint-add-section").click(function () {
        setupTour(AddSectionTour).start();


        $($(".show-sections")[0]).click(function () {
            setTimeout(AddSectionTour.next, 500);
            $($(".show-sections")[0]).off();
        });

        $("#add-section").click(function () {
            setTimeout(AddSectionTour.next, 500);
            $("#add-section").off();
        });
        //$(".input-money").change(function () {
        //    setTimeout(AddSectionTour.next, 500);
        //    $(".input-money").off();

        //    $(".cards-small").click(function () {
        //        AddSectionTour.next();
        //        $(".cards-small").off();
        //    });
        //});
    });
});