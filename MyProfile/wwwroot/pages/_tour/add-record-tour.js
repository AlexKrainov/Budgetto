var AddRecordTour;

$(function () {
    function setupTour(AddRecordTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        AddRecordTour.addStep({
            title: 'Шаг 1.',
            text: '<p>Нажмите на кнопку, чтобы добавить новую запись в таблицу.</p>',
            attachTo: { element: '#addRecord', on: 'left' },
            buttons: []
        });
        AddRecordTour.addStep({
            title: 'Шаг 2.',
            text: 'Введите сумму, которую хотите отразить в таблице (траты из одной категории можно записывать сразу через символ "+"). Нажмите Enter.',
            attachTo: { element: '.input-money', on: 'bottom' },
            buttons: []
        });
        AddRecordTour.addStep({
            title: 'Шаг 3.',
            text: 'Выберите категорию для суммы',
            attachTo: { element: '.cards-small', on: 'top' },
            buttons: []
        });
        AddRecordTour.addStep({
            title: 'Шаг 4.',
            text: 'Вы можете оставить комментарий для суммы или удалить категорию. Не покидая страницу, можно добавлять следующие суммы и выбирать для них категории.',
            attachTo: { element: '.records', on: 'bottom' },
            buttons: [{
                action: AddRecordTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        AddRecordTour.addStep({
            title: 'Шаг 5.',
            text: 'Нажимая кнопку добавить, вы сохраняйте все внесенные суммы в соответствующих категориях.',
            attachTo: { element: '.button-add-record', on: 'top' },
            buttons: [{
                action: AddRecordTour.next,
                classes: nextButtonClass,
                text: 'Спасибо, все понятно!'
            }]
        });

        return AddRecordTour;
    }

    AddRecordTour = new Shepherd.Tour({
        includeStyles: false,

        defaultStepOptions: {
            scrollTo: false,
            cancelIcon: {
                enabled: true
            }
        },
        useModalOverlay: true
    });

    $("#hint-add-record").click(function () {
        setupTour(AddRecordTour).start();

        $("#addRecord").click(function () {
            setTimeout(AddRecordTour.next, 800);
            $("#addRecord").off();
        });
        $(".input-money").change(function () {
            setTimeout(AddRecordTour.next, 500);
            $(".input-money").off();

            $(".cards-small").click(function () {
                AddRecordTour.next();
                $(".cards-small").off();
            });
        });
    });
});