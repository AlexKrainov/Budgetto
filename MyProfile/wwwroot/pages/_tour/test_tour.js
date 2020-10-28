var tour;

$(function () {
    function setupTour(tour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        tour.addStep({
            title: 'Title of first step',
            text: '<p>Content of first step</p><p><strong>Second</strong> line</p>',
            attachTo: { element: '#addRecord', on: 'left' },
            buttons: [{
                action: tour.cancel,
                classes: backButtonClass,
                text: 'Exit'
            }, {
                action: tour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        tour.addStep({
            title: 'Title of second step',
            text: 'Content of second step',
            attachTo: { element: '.input-money', on: 'bottom' },
            buttons: [{
                action: tour.back,
                classes: backButtonClass,
                text: 'Back'
            }, {
                action: tour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        tour.addStep({
            title: 'Title of third step',
            text: 'Content of third step',
            attachTo: { element: '.cards-small', on: 'top' },
            buttons: [{
                action: tour.back,
                classes: backButtonClass,
                text: 'Back'
            }, {
                action: tour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        tour.addStep({
            title: 'Title of fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '.records', on: 'bottom' },
            buttons: [{
                action: tour.back,
                classes: backButtonClass,
                text: 'Back'
            }, {
                action: tour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        tour.addStep({
            title: 'Title of fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '.button-add-record', on: 'top' },
            buttons: [{
                action: tour.back,
                classes: backButtonClass,
                text: 'Back'
            }, {
                action: tour.next,
                classes: nextButtonClass,
                text: 'Done'
            }]
        });

        //tour.addStep({
        //    title: 'Title of fifth step',
        //    text: 'Content of fifth step',
        //    attachTo: { element: '.button-close-record', on: 'top' },
        //    buttons: [{
        //        action: tour.back,
        //        classes: backButtonClass,
        //        text: 'Back'
        //    }, {
        //        action: tour.next,
        //        classes: nextButtonClass,
        //        text: 'Done'
        //    }]
        //});

        return tour;
    }

    tour = new Shepherd.Tour({
        includeStyles: false,

        defaultStepOptions: {
            scrollTo: false,
            cancelIcon: {
                enabled: true
            }
        },
        useModalOverlay: true
    });

    $("#shepherd-example").click(function () {
        setupTour(tour).start();
    });

    $("#addRecord").click(function () {
        setTimeout(tour.next, 800);
    });
    $(".input-money").change(function () {
        setTimeout(tour.next, 500);

        $(".cards-small").click(function () {
            tour.next();
        });
    });
});