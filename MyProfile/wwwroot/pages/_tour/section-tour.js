var SectionTour;

$(function () {
    function setupTour(SectionTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        SectionTour.addStep({
            title: 'Папка',
            text: `Для удобства вы можете группировать категории по папкам. Все папки расположены слева.
            Здесь вы можете создавать, редактировать и удалять папки`,
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
            title: 'Категории',
            text: 'Категорий может быть неограниченное количество',
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
            title: 'Показать категории',
            text: 'Чтобы посмотреть все категории в этой папке, нажмите на кнопку "Показать категории"',
            attachTo: { element: '.show-sections', on: 'right' },
            buttons: []
        });
        SectionTour.addStep({
            title: 'Категории',
            text: 'Категории, которые входят в выбранную папку, расположены справа. Здесь вы можете создавать, редактировать и удалять категории',
            attachTo: { element: '#section-vue .card-body', on: 'left' },
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