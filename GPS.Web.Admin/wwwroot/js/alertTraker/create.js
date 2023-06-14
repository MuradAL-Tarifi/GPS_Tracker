$(document).ready(function () {
    FlatPickerDate();
});

$(document).ajaxStart(function () { Pace.restart(); });
Vue.use(window.vuelidate.default);

var app = new Vue({
    el: '#app',
    data: {
        isEdit: false,
        InventoryId: 0,
        Inventories: [],
        InventorySelected: '',
        WarehouseId: 0,
        Warehouses: [],
        WarehouseSelected: '',
        savePending: false,
        isSensorExists: true,
        alertSensorView: JSON.parse(JSON.stringify(AlertSensorModel)),
        alertSensor: {
            inventoryId: 0,
            warehouseId: 0,
            minValueTemperature: 0,
            maxValueTemperature: 0,
            minValueHumidity: 0,
            maxValueHumidity: 0,
            toEmails: '',
            serial: '',
            userName: '',
        },
        alertBySensorId:0,
        isAlertSensorExists: false,
        hostName: hostName,
        returnURL: ReturnURL,
        fileName: '',
    },
    validations: {
        InventorySelected: {
            id: { required: function (value) { return value > 0; } }
        },
        WarehouseSelected: {
            id: { required: function (value) { return value > 0; } }
        },
        alertSensor: {
            minValueTemperature: { required: validators.required },
            maxValueTemperature: { required: validators.required },
            minValueHumidity: { required: validators.required },
            maxValueHumidity: { required: validators.required },
            toEmails: { required: validators.required },
            serial: {required: validators.required},
            userName: { required: validators.required },
        }
    },
    created: function () {
        debugger
        var self = this;
        self.GetWearhouses();
        self.alertSensor = self.alertSensorView;
        if (self.alertSensor.id > 0) {
            self.isEdit = true;
            self.alertSensor = self.alertSensorView;
            self.WarehouseSelected = { id: self.alertSensorView.warehouse.id, name: self.alertSensorView.warehouse.name };

            axios.get(hostName + '/Inventory/GetInventories?WarehouseId=' + self.alertSensorView.warehouse.id)
                .then(function (response) {
                    debugger
                    self.Inventories = response.data;
                })
                .catch(function (error) { });

            self.InventorySelected = { id: self.alertSensorView.inventory.id, name: self.alertSensorView.inventory.name };
        }
    },
    mounted: function () {
        var self = this;
        if (self.isEdit) {
            self.alertBySensorId = self.alertSensorView.id;
        }

    },
    methods: {
        GetWearhouses: function () {
            var self = this;
            axios.get(hostName + '/Warehouse/GetAllWarehouses/')
                .then(function (response) {
                    debugger
                    self.Warehouses = response.data.data;
                })
                .catch(function (error) { });
        },
        onSelectWarehouse: function () {
            debugger
            var self = this;
            axios.get(hostName + '/Inventory/GetInventories?WarehouseId=' + self.WarehouseSelected.id)
                .then(function (response) {
                    debugger
                    self.Inventories = response.data;
                })
                .catch(function (error) { });
        },
        resetSensor: function () {
            var self = this;
            self.savePending = false;

            /*  self.inventoryNumberExists = false;*/

            var emptyObj = {
                inventoryId: 0,
                warehouseId: 0,
                minValueTemperature: 0,
                maxValueTemperature: 0,
                minValueHumidity: 0,
                maxValueHumidity: 0,
                toEmails: '',
                serial: '',
                userName: '',
            };

            self.alertSensor = emptyObj;
            self.$v.InventorySelected.$reset();
            self.$v.WarehouseSelected.$reset();
        },
        onSaveAlertSensor: function () {
            debugger
            var self = this;

            self.$v.alertSensor.$touch();
            self.$v.WarehouseSelected?.$touch();
            self.$v.InventorySelected?.$touch();
            if (!self.$v.alertSensor.$invalid && !self.$v.WarehouseSelected?.$invalid && !self.$v.InventorySelected?.$invalid) {
                self.serverSideSensorValidation(self.alertSensor.serial).then((response1) => {
                    debugger
                    if (!response1.data) {
                        self.isSensorExists = false;
                        return;
                    }
                    if (!self.isSensorExists) {
                        return;
                    }
                    self.serverSideAlertBySensorValidation(self.alertSensor.serial).then((response) => {
                        debugger
                        if (response.data) {
                            self.isAlertSensorExists = true;
                            return;
                        }
                        if (self.isAlertSensorExists) {
                            return;
                        }
                        SwalConfirm("question", confirmSave, "", yes, cancel, function (result) {
                            self.savePending = true;
                            self.alertSensorView.warehouseId = self.WarehouseSelected.id;
                            self.alertSensorView.inventoryId = self.InventorySelected.id
                            axios({
                                method: 'post',
                                url: hostName + '/AlertTraker/Create',
                                data: self.alertSensorView,
                                headers: { 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                            }).then(function (response) {
                                window.location.href = self.returnURL;
                                //if (response.data.isSuccess) {
                                //    var cancelText = self.isEdit ? continueEdit : addNewSensor;
                                //    var successText = self.isEdit ? updateSuccess : addSuccess;
                                //    var body = '';

                                //    if (self.isEdit) {
                                //        if (response.data.errorList.length > 0) {
                                //            $.each(response.data.errorList, function (index, val) {
                                //                if (body != '') {
                                //                    body += '<br>';
                                //                }
                                //                body += val;
                                //            });
                                //        }
                                //    }

                                //    SwalOptions('success', successText, body, done, cancelText, function (result) {
                                //        if (result) {
                                //            window.location.href = self.returnURL;
                                //        }
                                //        else {
                                //            if (!self.isEdit) {
                                //                self.resetSensor();
                                //            }
                                //        }
                                //    });
                                // }
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
                });

            }


        },
        onDeleteSensor: function (index) {
            var self = this;
            self.sensorView.sensorsList.splice(index, 1);
            self.resetSensorView();
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
        
        serverSideAlertBySensorValidation: function (sensorSN) {
            debugger
            var x = hostName;
            return axios.get(hostName + `/AlertTraker/ValidateAlertBySensor?SensorSN=${sensorSN}`);
        },
        serverSideSensorValidation: function (sensorSN) {
            return axios.get(hostName + `/Sensors/ValidateSensor?SensorSN=${sensorSN}`);
        },
    }
});
