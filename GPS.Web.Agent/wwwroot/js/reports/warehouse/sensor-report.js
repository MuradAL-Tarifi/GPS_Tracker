var warehouseId;
var inventoryId;
var sensorSerial;
var pageSize = 100;
var fromDate = null;
var toDate = null;
var inventories = [];
var sensors = [];

$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    alertify.set('notifier', 'position', 'bottom-center');
    initFootable();
});

function getInventories(warehouseId) {
    if (warehouseId > 0) {
        axios.get(hostName + '/api/Inventories?warehouseId=' + warehouseId)
            .then(function (response) {
                var items = response.data;
                inventories = items;
                var s = '';
                if (items.length > 0) {
                    s += '<option value="">' + SelectInventoryText + '</option>';
                }
                $.each(items, function (index, item) {
                    s += '<option value="' + item.value + '">' + item.text + '</option>';
                });
                $("#InventoriesSelect").html(s);
                $('#InventoriesSelect').trigger('change');
            })
            .catch(function (error) {
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });
    }
    else {
        $("#InventoriesSelect").html("");
        $('#InventoriesSelect').trigger('change');
    }
}

function getSensors(inventoryId) {
    if (inventoryId > 0) {
        axios.get(hostName + '/api/InventorySensors?inventoryId=' + inventoryId)
            .then(function (response) {
                var items = response.data;
                sensors = items;
                var s = '';
                if (items.length > 0) {
                    s += '<option value="">' + SelectSensorText + '</option>';
                }
                $.each(items, function (index, item) {
                    s += '<option value="' + item.value + '">' + item.text + '</option>';
                });
                $("#SensorsSelect").html(s);
                $('#SensorsSelect').trigger('change');
            })
            .catch(function (error) {
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });
    }
    else {
        $("#SensorsSelect").html("");
        $('#SensorsSelect').trigger('change');
    }
}

function initFootable() {
    FooTable.init("#tbReports");
}

function PagedListSuccess() {
    initFootable();
}

function ChangeSize(size) {

    pageSize = size;
    GetData();
}

function Search() {
    pageSize = 100;
    GetData();
}

function GetData() {
    warehouseId = $("#WarehousesSelect").val();
    inventoryId = $("#InventoriesSelect").val();
    sensorSerial = $("#SensorsSelect").val();

    $.ajax({
        url: hostName + '/Reports/Warehouse',
        data: { warehouseId: warehouseId, inventoryId: inventoryId, sensorSerial: sensorSerial, fromDate: fromDate, toDate: toDate, show: pageSize },
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#PagedDataDiv').html(result);
            initFootable();
            Update();
        }
    });
}

function ExportToExcel() {
    inventoryId = $("#InventoriesSelect").val();
    sensorSerial = $("#SensorsSelect").val();

    axios.get(hostName + '/Reports/Warehouse/ExportToExcel/' + inventoryId + '/' + sensorSerial, {
        params: {
            fromDate: fromDate,
            toDate: toDate
        },
        responseType: 'blob', // important
    }).then(function (response) {
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', sensorSerial + '-InventorySensorReport-' + moment().format('YYYY-MM-DD') + '.xlsx');
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

function ExportToPDF() {
    inventoryId = $("#InventoriesSelect").val();
    sensorSerial = $("#SensorsSelect").val();

    axios.get(hostName + '/Reports/Warehouse/PrintPDF/' + inventoryId + '/' + sensorSerial, {
        params: {
            fromDate: fromDate,
            toDate: toDate
        },
        responseType: 'blob', // important
    }).then(function (response) {
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', sensorSerial + '-InventorySensorReport-' + moment().format('YYYY-MM-DD') + '.pdf');
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

// font-awesome 5 icons for datetimepicker
$.extend(true, $.fn.datetimepicker.defaults, {
    icons: {
        time: 'far fa-clock',
        date: 'far fa-calendar',
        up: 'fas fa-arrow-up',
        down: 'fas fa-arrow-down',
        previous: 'fas fa-chevron-left',
        next: 'fas fa-chevron-right',
        today: 'fas fa-calendar-check',
        clear: 'far fa-trash-alt',
        close: 'far fa-times-circle'
    }
});

var app = new Vue({
    el: '#app',
    data: {
        warehouseId: warehouseIdParam,
        inventoryId: inventoryIdParam,
        sensorSerial: sensorSerialParam,
        fromDate: null,
        toDate: null,
        dtpOptions: {
            format: 'YYYY/MM/DD h:mm A',
            useCurrent: 'day',
            showClear: true,
            showClose: true,
            showTodayButton: true
        },
        emptyResult: true,
    },
    created: function () {
        var self = this;
        window.Update = function () {
            self.emptyResult = $("#tbReports tbody").children()[0] == undefined;
        }

        self.fromDate = moment().subtract(1, "days").format("YYYY/MM/DD hh:mm A");
        self.toDate = moment().format("YYYY/MM/DD hh:mm A");

        self.warehouseId = $("#WarehousesSelect").val();
        self.inventoryId = $("#InventoriesSelect").val();
        self.sensorSerial = $("#SensorsSelect").val();
    },
    watch: {
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
        },
        fromDate: function (newValue, oldValue) {
            fromDate = newValue;
        },
        toDate: function (newValue, oldValue) {
            toDate = newValue;
        }
    },
    methods: {
        canSearch: function () {
            return this.warehouseId && this.inventoryId && this.sensorSerial && this.fromDate && this.toDate;
        },
    }
});