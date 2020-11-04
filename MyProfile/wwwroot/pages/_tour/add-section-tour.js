var AddSectionTour;

$(function () {
    function setupTour(AddSectionTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        AddSectionTour.addStep({
            title: 'Выбор папки',
            text: 'Для создании категории нажмите "показать категории".',
            attachTo: { element: '.show-sections', on: 'left' },
            buttons: [{
                action: AddSectionTour.cancel,
                classes: backButtonClass,
                text: 'Exit'
            }]
        });
        AddSectionTour.addStep({
            title: 'Кнопка "Добавить"',
            text: 'Нажмите кнопку "Добавить"',
            attachTo: { element: '#add-section', on: 'bottom' },
            buttons: []
        });
        AddSectionTour.addStep({
            title: 'Тип категории',
            text: 'Для начала выберите тип категории. Это могут быть раходы, доходы или инвестиции.',
            attachTo: { element: '.section-types', on: 'bottom' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Название',
            text: 'Введите название категории.',
            attachTo: { element: '#section-name', on: 'bottom' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Цвет',
            text: 'Нажмите на стрелку справа для выбора цвета категории.',
            attachTo: { element: '.choose-color', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Иконка',
            text: 'Выберите иконку.',
            attachTo: { element: '.choose-icons', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Сохранение',
            text: 'Нажмите кнопку сохранить',
            attachTo: { element: '#save-sections', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Done'
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