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
            FooTable.init("#tbAlerts");
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
            var self = this;
            $.ajax({
                url: hostName + "/Reports/Alert",
                data: {
                    show: self.pageSize,
                    page: self.pageNumber,
                    warehouseId: self.warehouseId,
                    inventoryId: self.inventoryId,
                    sensorId: self.sensorId,
                    alertType: self.alertTypeLookupId,
                    alertId: self.alertId,
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
        sendReadAlerts: function () {
            var alertIds = [];
            var rows = $("[id^=row_]");
            $.each(rows, function (index, row) {
                var id = row.id.split('_')[1];
                alertIds.push(id);
            });

            $.ajax({
                url: hostName + "/api/ReadAlerts",
                data: {
                    alertIds: alertIds
                },
                type: 'POST',
                cache: false,
                traditional:true,
                success: function (result) {
                    if (result.isSuccess) {
                        getNewNotifications();
                    }
                }
            });
        }
    },
    mounted: function () {
        var self = this;
        FlatPickerDate();
        self.getWarehoues();
        self.sendReadAlerts();
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