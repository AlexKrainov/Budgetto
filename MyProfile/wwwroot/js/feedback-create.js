var FeedbackVue = new Vue({
    el: "#feedback-vue",
    data: {
        feedback: {
            title: null,
            text: null,
            priority: 3,
            topic: "want",
            status: "new",
            images: [
                //{
                //    id: 0,
                //    imageBase64: null
                //}
            ]
        },

        imagesNumber: 0,
        isSaving: false,
        viewOne: true,
    },
    watch: {},
    mounted: function () { },
    methods: {
        encodeImageFileAsURL: function (event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onloadend = function () {
                FeedbackVue.feedback.images.push({
                    id: FeedbackVue.imagesNumber,
                    imageBase64: reader.result
                })
                FeedbackVue.imagesNumber++;
            }
            reader.readAsDataURL(file);
        },
        removeImage: function (img) {
            let index = this.feedback.images.findIndex(x => x.id == img.id);
            this.feedback.images.splice(index, 1);
        },
        save: function () {
            this.isSaving = true;

            return $.ajax({
                type: "Post",
                url: "/Feedback/SaveFeedback",
                contentType: "application/json",
                data: JSON.stringify(this.feedback),
                dataType: 'json',
                context: this,
                success: function (response) {

                    this.viewOne = false;
                    this.isSaving = false;
                }
            });

        }
    }
});

Vue.config.devtools = true;
