$(document).ready(function () {
    FlatPickerDate();
});

$(document).ajaxStart(function () { Pace.restart(); });
Vue.use(window.vuelidate.default);

var app = new Vue({
    el: '#app',
    data: {
        isEdit: false,
        brandId: null,
        brands: [],
        brandSelected: '',
        savePending: false,
        sensorView: JSON.parse(JSON.stringify(sensorModel)),
        sensor: {
            brandId: 0,
            brandName: '',
            calibrationDate: '',
            serial: '',
            name: '',
        },
        isSensorExists: false,
        tempSensorSN:'',
        hostName: hostName,
        returnURL: ReturnURL,
        createByUploadingExcel: false,
        progressUploadingFile: false,
        fileName: '',
        inventoryId:0
    },
    validations: {
        brandSelected: {
            id: {required: function (value) { return value > 0; }}
        },
        sensor: {
           // brandId: { required: function (value) { return value > 0; } },
            calibrationDate: { required: validators.required },
            serial: {
                required: validators.required
            },
            name: { required: validators.required },
        }
    },
    created: function () {
        var self = this;
        self.GetBrands();
        self.sensor = self.sensorView;
        if (self.sensor.id > 0) {
            self.isEdit = true;
            self.sensor = self.sensorView;
            self.brandSelected = { id: self.sensorView.brand.id, name: self.sensorView.brand.name };
        } else {
            self.sensorView.sensorsList = [];
        }
    },
    mounted: function () {
        var self = this;
        if (self.isEdit) {
            self.tempSensorSN = self.sensorView.serial;
        }
        const urlParams = new URLSearchParams(window.location.search);
        self.inventoryId = urlParams.get('inventoryId');
    },
    methods: {
        GetBrands: function () {
            var self = this;
            axios.get(hostName + '/Sensors/GetBrands/')
                .then(function (response) {
                    self.brands = response.data;
                    self.brandId = self.brands[0].Value;
                })
                .catch(function (error) { });
        },
        onSelectBrand: function () {
            var self = this;
            self.sensorView.brandId = self.brandSelected.id;
        },
        resetSensor: function () {
            var self = this;
            self.savePending = false;

          /*  self.inventoryNumberExists = false;*/

            var emptyObj = {
                brandId: 0,
                brandName: '',
                serial: '',
                name: '',
            };

            self.sensor = emptyObj;
            self.$v.brandSelected.$reset();
            self.$v.sensor.$reset();
        },
        onSaveSensor: function ()  {

            var self = this;
            //alert(self.sensor.serial);
            self.serverSideSensorValidation(self.sensor.serial).then((response) => {
                if (response.data) {
                    self.isSensorExists = true;
                    return;
                }
                self.checkSensorExists();
                if (self.isSensorExists) {
                    return;
                }
                self.$v.sensor.$touch();
                self.$v.brandSelected.$touch();
                //alert(self.$v.sensor.$invalid + " brandSelected" + self.$v.brandSelected.$invalid);
                if (!self.$v.sensor.$invalid && !self.$v.brandSelected.$invalid) {
                    if (self.sensorView.sensorsList == null) self.sensorView.sensorsList = [];
                    self.sensor.brandId = self.brandSelected.id;
                    self.sensor.brandName = self.brandSelected.name;
                    self.sensorView.sensorsList.push(JSON.parse(JSON.stringify(self.sensor)));
                    self.resetSensorView();
                }
            }).catch(function (error) { });
 
        },
        onDeleteSensor: function (index) {
            var self = this;
            self.sensorView.sensorsList.splice(index, 1);
            self.resetSensorView();
        },
        checkSensorExists: function ()  {
            var self = this;
            if (self.sensorView.sensorsList.filter(x => x.serial == self.sensorView.serial).length > 0) {
                self.isSensorExists = true;
            }
        },
        resetSensorView: function () {
            var self = this;
            self.savePending = false;
            //self.warehouse = JSON.parse(JSON.stringify(warehouseModel));
            self.sensorView.name = '';
            self.sensorView.serial = '';
            //self.sensorView.sensorsList = [];
            
            self.$v.sensor.$reset();
            self.$v.brandSelected.$reset();

            self.$v.$reset();
        },
        onSaveSensorView: function () {

            var self = this;
            self.serverSideSensorValidation(self.sensorView.serial).then((response) => {
                if (response.data && self.sensorView.serial != self.tempSensorSN) {
                    self.isSensorExists = true;
                    return;
                }
                SwalConfirm("question", confirmSave, "", yes, cancel, function (result) {
                    self.savePending = true;
                    axios({
                        method: 'post',
                        url: hostName + '/Sensors/Create?inventoryId=' + self.inventoryId,
                        data: self.sensorView,
                        headers: { 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                    }).then(function (response) {
                            if (response.data.isSuccess) {
                                var cancelText = self.isEdit ? continueEdit : addNewSensor;
                                var successText = self.isEdit ? updateSuccess : addSuccess;
                                var body = '';

                                if (self.isEdit) {
                                    if (response.data.errorList.length > 0) {
                                        $.each(response.data.errorList, function (index, val) {
                                            if (body != '') {
                                                body += '<br>';
                                            }
                                            body += val;
                                        });
                                    }
                                }

                                SwalOptions('success', successText, body, done, cancelText, function (result) {
                                    if (result) {
                                        window.location.href = self.returnURL;
                                    }
                                    else {
                                        if (!self.isEdit) {
                                            self.resetSensor();
                                        }
                                    }
                                });
                            }
                        })
                        .catch(function (error) {
                            if (error.response.data.errorList) {
                                SwalAlert("error", "", error.response.data.errorList[0]);
                            }
                            else {
                                SwalAlert("error", "", errorMessage);
                            }
                        }).finally(function () {
                            self.savePending = false;
                        });

                });
            }).catch(function (error) {

            });
                

        },
        serverSideSensorValidation: function (sensorSN) {
           return axios.get(hostName + `/Sensors/ValidateSensor?SensorSN=${sensorSN}`);
        },
        ExportDeaultExcelFile: function () {
            var fileName = "Default";
            $.ajax({
                url: hostName + '/Sensors/ExportDefaultExcelFile',
                type: 'GET',
                cache: false,
                xhr: function () {
                    var xhr = new XMLHttpRequest();
                    xhr.onreadystatechange = function () {
                        if (xhr.readyState == 2) {
                            if (xhr.status == 200) {
                                xhr.responseType = "blob";
                            } else {
                                xhr.responseType = "text";
                            }
                        }
                    };
                    return xhr;
                },
                success: function (data) {
                    const url = window.URL.createObjectURL(new Blob([data]));
                    const link = document.createElement('a');
                    link.href = url;
                    link.setAttribute("download", fileName + '.xlsx');
                    $("body").append(link);
                    link.click();
                    $("body").remove(link);
                }
            });
        },
        SelectExcelFile: function () {
            $("#fileInput").click();
        },
        FileUpload: function () {
            var self = this;
            self.fileName = $("#fileInput")[0].files.length ? $("#fileInput")[0].files[0].name : "";
        },
        ImportExcelFile: function () {
            var self = this;
            var fileInput = $("#fileInput")[0];
            if (fileInput.files.length > 0) {
                if (self.fileName.includes("Default")) {
                    self.progressUploadingFile = true;
                    setTimeout(function () { }, 300);
                    var formData = new FormData();
                    formData.append("ExcelFile", fileInput.files[0]);
                    $.ajax({
                        url: hostName + '/Sensors/UploadExcelFile',
                        type: 'POST',
                        contentType: false,
                        processData: false,
                        data: formData,
                        success: function (isSuccess) {
                            if (JSON.parse(isSuccess)) {
                                SwalFlipAlertOk('success', fileUploadedSuccessfully, '', function (result) {
                                    //console.log(result);
                                    if (result) {
                                        window.location.href = self.returnURL;
                                    }
                                });
                            } else {
                                alertify.notify(errorMessage, 'error').dismissOthers();
                            }
                            self.progressUploadingFile = false;
                        },
                        error: function (xhr, status, p3, p4) {
                            alertify.notify(errorMessage, 'error').dismissOthers();
                            self.progressUploadingFile = false;
                        }
                    });
                }
            }
            
        }
    }
});
