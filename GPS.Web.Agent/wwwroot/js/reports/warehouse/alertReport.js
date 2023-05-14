$(document).ajaxStart(function () { Pace.restart(); });
Vue.use(window.vuelidate.default);
// register globally
Vue.component('v-select', VueSelect.VueSelect)



var app = new Vue({
    el: "#app",
    data: {
            alertTypeLookupId: 0,
            warehouseId: null,
            inventoryId: null,
            sensorId: null,
            alertId: null,
            optionWarehouses: [],
            optionInventories: [],
            optionSensors: [],
            fromDate: null,
            toDate: null,
            pageNumber: null,
            pageSize: 100,
    },
    methods: {
        canSearch: function () {
            return (this.warehouseId || this.inventoryId || this.sensorId || this.alertTypeLookupId);
        },
        getWarehoues: async function () {
            var self = this;
            await axios.get(hostName + '/api/Warehouses').then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.optionWarehouses.push({ text: response.data[i].text, value: response.data[i].value });
                }

            }).catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });;
        }, getInventories: async function () {
            debugger;
            var self = this;
            await axios.get(hostName + '/api/Warehouses').then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.optionWarehouses.push({ text: response.data[i].text, value: response.data[i].value });
                }

            }).catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });;
        },
        onSelectWarehouse: function (obj) {
            debugger;
            var self = this;
            var selectedWarehouseId = obj != null ? obj.value : 0;
            self.getInventories(selectedWarehouseId);
        },
        onSelectInventory: function (obj) {
            var self = this;
            var selectedInventoryId = obj != null ? obj.value : 0;
            self.getSensors(selectedInventoryId);
        },
        getInventories: async function (warehouseId = 0) {
            var self = this;
            self.optionInventories = [];
            await axios.get(hostName + '/api/Inventories?warehouseId=' + (warehouseId > 0 ? warehouseId : self.warehouseId)).then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.optionInventories.push({ text: response.data[i].text, value: response.data[i].value });
                }

            }).catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });
        },
        getSensors: async function (inventoryId = 0) {
            var self = this;
            self.optionSensors = [];
            await axios.get(hostName + '/api/InventorySensors?inventoryId=' + (inventoryId > 0 ? inventoryId : self.inventoryId) + '&bySensorSerial=' + false).then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.optionSensors.push({ text: response.data[i].text, value: response.data[i].value });
                }

            }).catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });
        },
        initFootable: function () {
            FooTable.init("#tbtbAlertReport");
        },
        searchKeyUp: function (e) {
            var self = this;
            this.$nextTick(() => {
                if (e.keyCode == 13) {
                    self.search();
                }
            });
        },
        search: function () {
            debugger;
            var self = this;
            $.ajax({
                url: hostName + "/Reports/AlertReport",
                data: {
                    show: self.pageSize,
                    page: self.pageNumber,
                    warehouseId: self.warehouseId != undefined ? self.warehouseId.value : "",
                    inventoryId: self.inventoryId != undefined ? self.inventoryId.value : "",
                    sensorId: self.sensorId != undefined ? self.sensorId.value : "",
                    fromDate: self.fromDate,
                    toDate: self.toDate
                },
                type: 'GET',
                cache: false,
                success: function (result) {
                    $('#PagedDataDiv').html(result);
                    self.initFootable();
                }
            });
        },
        ExportToExcelData: function() {
            debugger;
            var self = this;
            var x = self.warehouseId != undefined ? self.warehouseId.value : null;
            var x1 = self.inventoryId != undefined ? self.inventoryId.value : null;
            var x2 = self.sensorId != undefined ? self.sensorId.value : null;
            var x3 = self.fromDate != null ? self.fromDate : null;
            var x4 = self.toDate != null ? self.toDate : null;

            axios({
                method: 'get',
                url: hostName + '/Reports/Sensor/ExportAlertReport/' + x + '/' + x1 + '/' + x2 + '/' + x3 + '/' + x4,
                responseType: 'blob', // important
            }).then(function (response) {

                const url = window.URL.createObjectURL(new Blob([response.data]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', 'AlertReport' + '.xlsx');
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            })
                .catch(function (error) {
                    if (error.response.status != 404) {
                        alertify.notify(ErrorMessage, 'error').dismissOthers();
                    }
                });
        }
    },
    mounted: function () {
        var self = this;
        FlatPickerDate();
        self.getWarehoues();
        const urlParams = new URLSearchParams(window.location.search);
        self.warehouseId = urlParams.get('warehouseId');
        self.inventoryId = urlParams.get('inventoryId');
        self.sensorId = urlParams.get('sensorId');
        self.alertTypeLookupId = urlParams.get('alertType');
        self.alertId = urlParams.get('alertId');
        if (self.warehouseId > 0 && self.inventoryId > 0 && self.sensorId > 0 && self.alertTypeLookupId > 0 && self.alertId > 0) {
            self.search();
            self.alertId = null;
        }
    },
    created: function () {
        var self = this;
        window.ChangeSize = function (size) {
            self.pageSize = size;
            setTimeout(function () { self.sendReadAlerts(); }, 2000);
            self.search();
        }
        window.PagedListSuccess = function () {
            self.initFootable();
            self.sendReadAlerts();
        }
    }
});