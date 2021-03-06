var SectionTour;

$(function () {
    if (window.layoutHelpers.isSmallScreen()) {
        return;
    }
    function setupTour(SectionTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        SectionTour.addStep({
            title: 'Группа категорий',
            text: `Для удобства вы можете группировать категории по группам. Все группы категорий расположены слева.
            Здесь вы можете создавать, редактировать и удалять группу категорий`,
            attachTo: { element: '.area-item', on: 'right' },
            buttons: [{
                action: SectionTour.cancel,
                classes: backButtonClass,
                text: 'Завершить'
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
            text: 'Чтобы посмотреть все категории в этой группе, нажмите на кнопку "Показать категории"',
            attachTo: { element: '.show-sections', on: 'right' },
            buttons: []
        });
        SectionTour.addStep({
            title: 'Категории',
            text: 'Категории, которые входят в выбранную группу, расположены справа. Здесь вы можете создавать, редактировать и удалять категории',
            attachTo: { element: '#section-vue .card-body', on: 'left' },
            buttons: [{
                action: SectionTour.cancel,
                classes: nextButtonClass,
                text: 'Завершить'
            }]
        });
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

    SectionTour.on('cancel', () => {
        $($(".show-sections")[0]).off();
    });

    $("#hint-show-page").click(function () {
        setupTour(SectionTour).start();

        $($(".show-sections")[0]).click(function () {
            setTimeout(SectionTour.next, 500);
            $($(".show-sections")[0]).off();
        });
    });
});