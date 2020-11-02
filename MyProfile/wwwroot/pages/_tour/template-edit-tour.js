var TemplateTour;

$(function () {
    function setupTour(TemplateTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        TemplateTour.addStep({
            title: 'First',
            text: '<p>Для удоства категории разбиты на папки</p>',
            attachTo: { element: '#template-period-container', on: 'right' },
            buttons: [{
                action: TemplateTour.cancel,
                classes: backButtonClass,
                text: 'Выход'
            }, {
                action: TemplateTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        TemplateTour.addStep({
            title: 'Second',
            text: 'Content of second step',
            attachTo: { element: '.cards-medium', on: 'right' },
            buttons: [{
                action: TemplateTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: TemplateTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        TemplateTour.addStep({
            title: 'Third',
            text: 'Content of third step',
            attachTo: { element: '.show-sections', on: 'right' },
            buttons: []
        });
        TemplateTour.addStep({
            title: 'Fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '#section-vue .card-body', on: 'top' },
            buttons: [{
                action: TemplateTour.next,
                classes: nextButtonClass,
                text: 'Завершить'
            }]
        });
        //TemplateTour.addStep({
        //    title: 'Fiftith',
        //    text: 'Content of fourth step',
        //    attachTo: { element: 'a[href="/Section/Edit"]', on: 'right' },
        //    buttons: [{
        //        action: TemplateTour.back,
        //        classes: backButtonClass,
        //        text: 'Назад'
        //    }, {
        //        action: TemplateTour.next,
        //        classes: nextButtonClass,
        //        text: 'Далее'
        //    }]
        //});
        //TemplateTour.addStep({
        //    title: 'Sixinth',
        //    text: 'Content of fourth step',
        //    attachTo: { element: 'a[href="/Template/List"]', on: 'right' },
        //    buttons: [{
        //        action: TemplateTour.back,
        //        classes: backButtonClass,
        //        text: 'Назад'
        //    }, {
        //        action: TemplateTour.next,
        //        classes: nextButtonClass,
        //        text: 'Далее'
        //    }]
        //});
        //TemplateTour.addStep({
        //    title: 'Seventith',
        //    text: 'Content of fourth step',
        //    attachTo: { element: '.theme-settings-open-btn2 i', on: 'left' },
        //    buttons: [{
        //        action: TemplateTour.back,
        //        classes: backButtonClass,
        //        text: 'Назад'
        //    }, {
        //        action: TemplateTour.next,
        //        classes: nextButtonClass,
        //        text: 'Завершить'
        //    }]
        //});

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

    $("#hint-show-page").click(function () {
        setupTour(TemplateTour).start();

        $($(".show-sections")[0]).click(function () {
            setTimeout(TemplateTour.next, 500);
            $($(".show-sections")[0]).off();
        });
    });
});