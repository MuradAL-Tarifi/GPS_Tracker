
$(document).ajaxStart(function () { Pace.restart(); });

var app = new Vue({
    el: '#app',
    data: {
        fromDate: null,
        toDate: null,
        reportData: null,
        groupUpdatesByType: '',
        groupUpdatesValue: 1,
        loading: false,
        colSpan: 2,
        groupUpdatesByTypeValue: '',
        sensorsSN: [],
        warehouses: [],
        inventories:[],
        sensorsSNText:''
    }, computed: {
        emptyResult() {
            return !this.reportData || this.reportData.length == 0;
        }
    }, mounted: function () {
        var self = this;
        FlatPickerDateTime();
        self.initWarehousesInventoriesSensorsJsTree();
    }, created: function () {
        alertify.set('notifier', 'position', 'bottom-center');
        var self = this;

        self.fromDate = moment().subtract(1, "days").format("YYYY/MM/DD hh:mm A");
        self.toDate = moment().format("YYYY/MM/DD hh:mm A");
        window.handelJsTreeData = function (data) {
            //var self = this;
            app.sensorsSN = [];
            var i, j;
            for (i = 0, j = data.selected.length; i < j; i++) {
                var id = data.instance.get_node(data.selected[i]).id;
                if (id.includes("_sensor")) {
                    app.sensorsSN.push(id.split("_sensor")[0]);
                }
            }
            app.sensorsSNText = app.sensorsSN.toString();

            //$("#selectedSensors").val(app.sensorsSN.toString());
            //console.log(self.sensorsSNText);
        }
    }, methods: {
        initWarehousesInventoriesSensorsJsTree: function () {
            if (WarehousesInventoriesSensors != null) {
                $("#jstree-warehouses-inventories-sensors").jstree({
                    core: {
                        data: WarehousesInventoriesSensors
                    },
                    plugins: ['types', 'checkbox', 'wholerow'],
                    types: {
                        default: {
                            icon: 'far fa-folder'
                        },
                        warehouse: {
                            icon: 'fa fa-warehouse text-success'
                        },
                        inventory: {
                            icon: 'fa fa-boxes text-warning'
                        },
                        sensor: {
                            icon: 'fa fa-microchip text-info'
                        },

                    }
                });

                $('#jstree-warehouses-inventories-sensors').on('changed.jstree', function (e, data) {
                    handelJsTreeData(data);
                });

                $('#selectedSensors').focus(function () {
                    $("#selectNodeTree").animate({ height: 200 }, 200).css({ display: "block" });
                });
                $(document).mousedown(function (e) {
                    var element = e.target;
                    if ((!element.classList.value.includes("jstree"))) {
                        setTimeout(function () { $("#selectNodeTree").animate({ height: 0 }, 200).css({ display: "none" }); }, 200);
                    }
                });
            }
        },
        canSearch: function () {
            return this.sensorsSN.length > 0 && this.fromDate && this.toDate;
        },
        exportToPDF: function () {
            var self = this;
            showLoader();
            self.loading = true;
            self.reportData = null;
            self.colSpan = "2";
            self.groupUpdatesByTypeValue = '';
            self.warehouses = [];
            self.inventories = [];

            axios({
                method: 'post',
                url: hostName + '/Reports/WarehouseInventorySensorReport/TemperatureAndHumiditySensorsHistoryReport/PrintPDF',
                responseType: 'blob', // important
                data: {
                    lsSensorSerials: self.sensorsSN,
                    fromDate: self.fromDate,
                    toDate: self.toDate,
                    groupUpdatesByType: self.groupUpdatesByType,
                    groupUpdatesValue: self.groupUpdatesValue
                }
            }).then(function (response) {
                //console.log(response.data);
                let blob = new Blob([response.data], { type: "application/pdf" });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', 'InventoryReport-' + moment().format('YYYY-MM-DD') + '.pdf');
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                hideLoader();
            })
                .catch(function (error) {
                    if (error.response.status != 404) {
                        alertify.notify(ErrorMessage, 'error').dismissOthers();
                    }
                    self.loading = false;
                    setTimeout(function () {
                        hideLoader();
                    }, 1400);
                });
        },
        search: function () {
            showLoader();
            var self = this;
            self.loading = true;
            self.reportData = null;
            self.colSpan = "2";
            self.groupUpdatesByTypeValue = '';
            self.warehouses = [];
            self.inventories = [];
            axios({
                method: 'post',
                url: hostName + '/Reports/WarehouseInventorySensorReport/TemperatureAndHumiditySensorsHistoryReport',
                headers: {},
                data: {
                    lsSensorSerials: self.sensorsSN,
                    fromDate: self.fromDate,
                    toDate: self.toDate,
                    groupUpdatesByType: self.groupUpdatesByType,
                    groupUpdatesValue: self.groupUpdatesValue
                }
            }).then(function (response) {
                if (response.data != null) {
                    self.reportData = response.data;
                    self.warehouses = response.data.map(obj => ({ warehouseId: obj.warehouseId, warehouseName: obj.warehouseName }))
                        .filter(
                            (elem, index, arr) => index === arr.findIndex((obj) => obj.warehouseId === elem.warehouseId)
                    );
                    self.inventories = response.data.map(obj => ({ inventoryId: obj.inventoryId, inventoryName: obj.inventoryName, warehouseId: obj.warehouseId }))
                        .filter(
                            (elem, index, arr) => index === arr.findIndex((obj) => obj.inventoryId === elem.inventoryId)
                        );
                    self.colSpan = self.groupUpdatesByType ? "1" : "2";
                    self.groupUpdatesByTypeValue = self.groupUpdatesByType;
                    //console.log(self.warehouses);
                    //console.log(self.inventories);
                }
                self.loading = false;
                hideLoader();
            }).catch(function (error) {
                if (error.response.status != 404) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                }
                hideLoader();
            }).finally(function () {
                self.loading = false;
                setTimeout(function () {
                    hideLoader();
                }, 1400);
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
        },
    }
});


