$(document).ajaxStart(function () { Pace.restart(); });

var app = new Vue({
    el: '#app',
    data: {
        warehouses: [],
        inventories: [],
        sensors: [],
        warehouseId: '',
        inventoryId: '',
        sensorSerial: '',
        fromDate: null,
        toDate: null,
        pageSize: 100,
        reportData: null,
        emptyResult: true,
        groupUpdatesByType: '',
        groupUpdatesValue: 1,
        loading: false,
    },
    created: function () {
        alertify.set('notifier', 'position', 'bottom-center');

        var self = this;

        window.ChangeSize = function (size) {
            self.pageSize = size;
            self.search();
        }

        window.PagedListSuccess = function () {
            //self.initFootable();
        }

        self.fromDate = moment().subtract(1, "days").format("YYYY/MM/DD hh:mm A");
        self.toDate = moment().format("YYYY/MM/DD hh:mm A");
    },
/*    watch: {
        warehouseId: function (newValue, oldValue) {
            var self = this;
            if (newValue != oldValue) {
                self.inventoryId = '';
                self.sensorSerial = '';
            }
        },
        inventoryId: function (newValue, oldValue) {
            var self = this;
            if (newValue != oldValue) {
                self.sensorSerial = '';
            }
        }
    },*/
    mounted: async function () {
        FlatPickerDateTime();
        var self = this;
        self.getWarehouses();
        const urlParams = new URLSearchParams(window.location.search);
        self.warehouseId = urlParams.get('warehouseId');
        if (self.warehouseId > 0) {
            await self.getInventories();
        }
        self.inventoryId = urlParams.get('inventoryId');
        if (self.inventoryId > 0) {
            self.sensorSerial = urlParams.get('sensorSerial');
            await self.getSensors(urlParams.get('sensorSerial'));
        }
        
    },
    methods: {
        canSearch: function () {
            return this.warehouseId && this.inventoryId && this.sensorSerial && this.fromDate && this.toDate;
        },
        getWarehouses: function () {
            var self = this;
            self.inventoryId = '';
            axios.get(hostName + '/api/Warehouses')
                .then(function (response) {
                    self.warehouses = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        getInventories: function () {
            var self = this;
            self.inventoryId = '';
            self.sensorSerial = '';
            axios.get(hostName + '/api/Inventories?warehouseId=' + self.warehouseId)
                .then(function (response) {
                    self.inventories = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        getSensors: function () {
            var self = this;
            axios.get(hostName + '/api/InventorySensors?inventoryId=' + self.inventoryId + "&sensorSerial=" + self.sensorSerial)
                .then(function (response) {
                    self.sensors = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        search: function () {
            var self = this;
            self.loading = true;
            showLoader();
            $.ajax({
                url: hostName + '/Reports/InventorySensor',
                data: {
                    warehouseId: self.warehouseId,
                    inventoryId: self.inventoryId,
                    sensorSerial: self.sensorSerial,
                    fromDate: self.fromDate,
                    toDate: self.toDate,
                    show: self.pageSize,
                    groupUpdatesByType: self.groupUpdatesByType,
                    groupUpdatesValue: self.groupUpdatesValue
                },
                type: 'GET',
                cache: false,
                success: function (result) {
                    $('#PagedDataDiv').html(result);
                    //self.initFootable();
                    self.update();
                    self.loading = false;
                    setTimeout(function () {
                        hideLoader();
                    }, 1400);
                },
                error: function (error) {
                    self.loading = false;
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                    hideLoader();
                }
            });
        },
        update: function () {
            var self = this;
            self.emptyResult = $("#tbReports tbody").children()[0] == undefined;
        },
        exportToPDF: function () {
            var self = this;
            axios.get(hostName + '/Reports/InventorySensor/PrintPDF/' + self.inventoryId + '/' + self.sensorSerial, {
                params: {
                    fromDate: self.fromDate,
                    toDate: self.toDate,
                    groupUpdatesByType: self.groupUpdatesByType,
                    groupUpdatesValue: self.groupUpdatesValue
                },
                responseType: 'blob', // important
            }).then(function (response) {
                const url = window.URL.createObjectURL(new Blob([response.data]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', self.sensorSerial + '-InventorySensorReport-' + moment().format('YYYY-MM-DD') + '.pdf');
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            })
                .catch(function (error) {
                    if (error.response.status != 404) {
                        alertify.notify(ErrorMessage, 'error').dismissOthers();
                    }
                });
        },
        groupUpdatesValueChanged: function () {
            if (this.groupUpdatesValue < 0) {
                this.groupUpdatesValue = this.groupUpdatesValue * -1;
            }

            if (this.groupUpdatesValue.length > 2) {
                this.groupUpdatesValue = this.groupUpdatesValue.substring(0, 2);
            }

            if (this.groupUpdatesByType == 'day' && this.groupUpdatesValue > 31) {
                this.groupUpdatesValue = 31;
            }
            else if (this.groupUpdatesByType == 'hour' && this.groupUpdatesValue > 24) {
                this.groupUpdatesValue = 24;
            }
        }
    }
});