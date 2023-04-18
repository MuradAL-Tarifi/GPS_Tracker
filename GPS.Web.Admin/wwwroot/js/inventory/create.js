$(document).ajaxStart(function () { Pace.restart(); });
Vue.use(window.vuelidate.default);
Vue.component('v-select', VueSelect.VueSelect);

var app = new Vue({
    el: '#app',
    data: {
        isEdit: false,
        agentId: null,
        fleetId: null,
        agents: [],
        fleets: [],
        sensors: [],
        warehouses: [],
        gateways: [],
        savePending: false,
        inventory: JSON.parse(JSON.stringify(inventoryModel)),
        inventorySensor: {
            inventoryId: 0,
            serial: '',
            sensorView:null,
        },
        sensorView: {
            id: 0,
            serial: '',
        },
        selectedFleet: null,
        selectedSensor: null,
        hostName: hostName,
        inventoryNumberExists: false,
        sensorAlreadyRegisteredWithAnotherInventory: false,
        returnURL: ReturnURL
    },
    validations: {
        fleetId: { required: function (value) { return value > 0; } },
        inventory: {
            warehouseId: { required: function (value) { return value > 0; } },
            gatewayId: { required: function (value) { return value > 0; } },
            name: { required: validators.required },
            /*inventoryNumber: { required: validators.required },*/
            /*sfdaStoringCategory: { required: validators.required },*/
        },
        inventorySensor: {
            sensorId: { required: validators.required },
        },
        sensorView: {
            id: { required: validators.required },
            serial: { required: validators.required },
        }
    },
    created: function () {
        var self = this;
        self.GetAgents();
        if (self.inventory.id > 0) {
            self.isEdit = true;
            self.GetSensorsByGateway();
        } else {
            self.inventory.inventorySensors = [];
        }
    },
    mounted: function () {
        var self = this;
        axios.get(hostName + '/Inventory/GetGateways')
            .then(function (response) {
                self.gateways = response.data;
            })
            .catch(function (error) { });
    },
    methods: {
        GetAgents: function () {
            var self = this;
            axios.get(hostName + '/Inventory/GetAgents/')
                .then(function (response) {
                    self.agents = response.data;
                    self.agentId = self.agents[0].Value;
                    self.GetFleets();
                })
                .catch(function (error) { });
        },
        GetFleets: function () {
            var self = this;
            axios.get(hostName + '/Inventory/GetFleets?AgentId=' + self.agentId)
                .then(function (response) {
                    self.fleets = response.data;
                    self.selectedFleet = null;
                    self.fleets.map(function (x) {
                        return x.fleetSelectLabel = x.Text;
                    });
                   
                    self.fleetId = self.fleets[0].Value;
                    self.selectedFleet = self.fleets[0];
                    if (self.isEdit) {
                        self.fleetId = self.inventory.warehouse.fleetId;
                        self.selectedFleet = self.fleets.filter(x => x.Value == self.fleetId)[0]
                    }

                    self.GetWarehouses();

                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        GetSensorsByGateway: function () {
            var self = this
            if (self.inventory.gatewayId <= 0) return;

            axios.get(hostName + '/Inventory/GatSensorByGateway?GatewayId=' + self.inventory.gatewayId)
                .then(function (response) {
                    self.sensors = response.data;
                    self.selectedSensor = null;
                    self.sensors.map(function (x) {
                        return x.sensorSelectLabel = x.Text;
                    });
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        selectFleet: function () {
            var self = this;
            self.selectedFleet = this.fleet.filter(x => x.Value == self.fleetId)[0];
        },
        GetWarehouses: function () {
            var self = this;
            self.fleetId = self.selectedFleet.Value;
            axios.get(hostName + '/Inventory/GetWarehouses?AgentId=' + self.agentId + '&fleetId=' + self.fleetId)
                .then(function (response) {
                    self.warehouses = response.data;

                    if (self.warehouses.filter(x => x.Value == self.warehouse.warehouseId).length == 0) {
                        self.warehouse.warehouseId = 0;
                    }
              
                })
                .catch(function (error) { });
        },
        isInventoryNumberExists: function () {
            var self = this;
            self.inventoryNumberExists = false;
            if (self.inventory.inventoryNumber) {
                axios.get(hostName + '/Inventory/isInventoryNumberExists?inventoryNumber=' + self.inventory.inventoryNumber + '&fleetId=' + self.fleetId)
                    .then(function (response) {
                        self.inventoryNumberExists = response.data.data;
                    })
                    .catch(function (error) { });
            }
        },
        resetSensor: function () {
            var self = this;
            self.savePending = false;

            self.inventoryNumberExists = false;

            var emptyObj = {
                inventoryId: 0,
                serial: '',
                sensorView: null,
            };

            self.inventorySensor = emptyObj;
            self.$v.inventorySensor.$reset();
        },
        onSaveSensor: function () {
            var self = this;
            self.serverSideInventorySensorValidation(self.selectedSensor.Value).then((response) => {
                if (response.data) {
                    self.sensorAlreadyRegisteredWithAnotherInventory = true;
                    return;
                }
                self.sensorView.serial = self.selectedSensor.Text;
                self.sensorView.id = self.selectedSensor.Value;
                self.sensorCheckExists();
                if (self.sensorExists) {
                    return;
                }
                self.inventorySensor = {
                    sensorView: JSON.parse(JSON.stringify(self.sensorView)),
                    serial: self.sensorView.serial,
                    sensorId: self.sensorView.id,
                }

                self.$v.inventorySensor.$touch();
                if (!self.$v.inventorySensor.$invalid) {
                    self.inventory.inventorySensors.push(JSON.parse(JSON.stringify(self.inventorySensor)));
                    self.resetSensor();
                }
            }).catch(function (error) { });

        },
        sensorCheckExists: function () {
            var self = this;
            if (self.inventory.inventorySensors.filter(x => x.sensorView.id == self.sensorView.id).length > 0) {
                self.sensorExists = true;
            } else {
                self.sensorExists = false;
            }
        },
        onDeleteSensor: function (index) {
            var self = this;
            self.inventory.inventorySensors.splice(index, 1);
            self.resetSensor();
        },
        resetInventory: function () {
            var self = this;
            self.savePending = false;

            //self.warehouse = JSON.parse(JSON.stringify(warehouseModel));
            self.inventory.name = '';
            self.inventory.inventoryNumber = '';
            self.inventory.sfdaStoringCategory = '';
            self.inventory.inventorySensors = [];

            self.$v.$reset();
        },
        onSaveInventory: function () {
            var self = this;
            //console.log(self.fleetId);
            self.$v.fleetId.$touch();
            self.$v.inventory.$touch();
            if (!self.$v.inventory.$invalid && !self.$v.fleetId.$invalid) {
                self.$v.$reset();
                SwalConfirm("question", confirmSave, "", yes, cancel, function (result) {
                    self.savePending = true;
                    axios({
                        method: 'post',
                        url: hostName + '/Inventory/Create',
                        data: self.inventory,
                        headers: { 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                    })
                        .then(function (response) {
                            if (response.data.isSuccess) {
                                var cancelText = self.isEdit ? continueEdit : addNewInventory;
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
                                    if (result) { // done
                                        //window.location.href = hostName + "/Inventory/";
                                        window.location.href = self.returnURL;
                                    }
                                    else {

                                        // reset
                                        if (!self.isEdit) {
                                            self.resetInventory();
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
            }
        },
        serverSideInventorySensorValidation: function (sensorId) {
            return axios.get(hostName + `/Inventory/ValidateInventorySensor?SensorId=${sensorId}`);
        }
    }
});
