var AddSectionTour;

$(function () {
    function setupTour(AddSectionTour) {
        var backButtonClass = 'btn btn-sm btn-secondary md-btn-flat';
        var nextButtonClass = 'btn btn-sm btn-primary';
        var isRtl = $('html').attr('dir') === 'rtl';

        AddSectionTour.addStep({
            title: 'Title of first step',
            text: '<p>Content of first step</p><p><strong>Second</strong> line</p>',
            attachTo: { element: '.show-sections', on: 'left' },
            buttons: [{
                action: AddSectionTour.cancel,
                classes: backButtonClass,
                text: 'Exit'
            }]
        });
        AddSectionTour.addStep({
            title: 'Title of second step',
            text: 'Content of second step',
            attachTo: { element: '#add-section', on: 'bottom' },
            buttons: []
        });
        AddSectionTour.addStep({
            title: 'Title of third step',
            text: 'Content of third step',
            attachTo: { element: '.section-type', on: 'bottom' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Title of fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '#section-name', on: 'bottom' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Title of fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '.choose-color', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Title of fourth step',
            text: 'Content of fourth step',
            attachTo: { element: '.choose-icons', on: 'top' },
            buttons: [{
                action: AddSectionTour.next,
                classes: nextButtonClass,
                text: 'Next'
            }]
        });
        AddSectionTour.addStep({
            title: 'Title of fourth step',
            text: 'Content of fourth step',
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