var SectionTour;

$(function () {
    function setupTour(SectionTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        SectionTour.addStep({
            title: 'First',
            text: '<p>Для удоства категории разбиты на папки</p>',
            attachTo: { element: '.area-item', on: 'right' },
            buttons: [{
                action: SectionTour.cancel,
                classes: backButtonClass,
                text: 'Выход'
            }, {
                action: SectionTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        SectionTour.addStep({
            title: 'Second',
            text: 'Content of second step',
            attachTo: { element: '.cards-medium', on: 'right' },
            buttons: [{
                action: SectionTour.back,
                classes: backButtonClass,
                text: 'Назад'
            }, {
                action: SectionTour.next,
                classes: nextButtonClass,
                text: 'Далее'
            }]
        });
        SectionTour.addStep({
            title: 'Third',
            text: 'Content of third step',
            attachTo: { element: '.show-sections', on: 'right' },
            buttons: []
        });
        SectionTour.addStep({
            title: 'Fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '#section-vue .card-body', on: 'top' },
            buttons: [{
                action: SectionTour.next,
                classes: nextButtonClass,
                text: 'Завершить'
            }]
        });
        //SectionTour.addStep({
        //    title: 'Fiftith',
        //    text: 'Content of fourth step',
        //    attachTo: { element: 'a[href="/Section/Edit"]', on: 'right' },
        //    buttons: [{
        //        action: SectionTour.back,
        //        classes: backButtonClass,
        //        text: 'Назад'
        //    }, {
        //        action: SectionTour.next,
        //        classes: nextButtonClass,
        //        text: 'Далее'
        //    }]
        //});
        //SectionTour.addStep({
        //    title: 'Sixinth',
        //    text: 'Content of fourth step',
        //    attachTo: { element: 'a[href="/Template/List"]', on: 'right' },
        //    buttons: [{
        //        action: SectionTour.back,
        //        classes: backButtonClass,
        //        text: 'Назад'
        //    }, {
        //        action: SectionTour.next,
        //        classes: nextButtonClass,
        //        text: 'Далее'
        //    }]
        //});
        //SectionTour.addStep({
        //    title: 'Seventith',
        //    text: 'Content of fourth step',
        //    attachTo: { element: '.theme-settings-open-btn2 i', on: 'left' },
        //    buttons: [{
        //        action: SectionTour.back,
        //        classes: backButtonClass,
        //        text: 'Назад'
        //    }, {
        //        action: SectionTour.next,
        //        classes: nextButtonClass,
        //        text: 'Завершить'
        //    }]
        //});

        return SectionTour;
    }

    SectionTour = new Shepherd.Tour({
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
        setupTour(SectionTour).start();

        $($(".show-sections")[0]).click(function () {
            setTimeout(SectionTour.next, 500);
            $($(".show-sections")[0]).off();
        });
    });
});