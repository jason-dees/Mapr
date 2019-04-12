function setUpNewMapForm(newMapUrl) {
    var newMap = new Vue({
        el: "#newMapModal",
        data: {
            mapName: "",
            image: null,
            imagePreview: "",
            nameErrorMessage: "",
            formErrorMessage: ""
        },
        methods: {
            submit: function (event) {
                let self = this;

                if (this.checkIsEmpty()) return;
                if (self.image == null) return;

                let formData = new FormData();
                formData.append("ImageData", self.image);
                formData.set("Name", self.mapName);

                const config =  { headers: {'Content-Type': 'multipart/form-data' }};
                axios({
                    method: 'post',
                    url: newMapUrl,
                    data: formData,
                    config: config
                }).then(function (response) {
                    self.formErrorMessage = "";
                }).catch(function (error) {
                    self.formErrorMessage = error.message;
                });
            },
            checkIsEmpty: function () {
                if (this.mapName.trim().length == 0) {
                    this.nameErrorMessage = "Map needs a name!";
                    return true;
                }
                else {
                    this.nameErrorMessage = "";
                    return false;
                }
            },
            fileSelected: function(event){
                let self = this;
                self.imagePreview = "";
                self.image = null;

                if(event.target.files.length == 0) return;
                self.image = event.target.files[0];

                let fileReader = new FileReader();
                fileReader.addEventListener('load', function(){
                    self.imagePreview = fileReader.result;
                }, false);
                fileReader.readAsDataURL(self.image);
            }
        }
    });
}
