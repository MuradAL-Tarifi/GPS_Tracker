
$(document).ajaxStart(function () { Pace.restart(); });
//Vue.use(window.vuelidate.default);
// register globally
Vue.component('v-select', VueSelect.VueSelect)

var app = new Vue({
    el: '#app',
    data: {
        warehouses: [],
        inventories: [],
        warehouseId: '',
        inventoryId: '',
        fromDate: null,
        toDate: null,
        reportData: null,
        selectedInventoryName: null,
        groupUpdatesByType: '',
        groupUpdatesValue: 1,
        loading: false,
        colSpan: 2,
        groupUpdatesByTypeValue: '',
        selectedSensors: [],
        optionSensors: [],
        sensorsSN: []
    },
    watch: {
        warehouseId: function (newValue, oldValue) {
            var self = this;
            if (newValue != oldValue) {
                self.inventoryId = '';
            }
        },
    },
    computed: {
        emptyResult() {
            return !this.reportData || this.reportData.length == 0;
        }
    },
    created: function () {
        alertify.set('notifier', 'position', 'bottom-center');
        var self = this;

        self.fromDate = moment().subtract(1, "days").format("YYYY/MM/DD hh:mm A");
        self.toDate = moment().format("YYYY/MM/DD hh:mm A");
    },
    mounted: function () {
        var self = this;
        axios.get(hostName + '/api/Warehouses')
            .then(function (response) {
                self.warehouses = response.data;
            })
            .catch(function (error) {
                alertify.notify(ErrorMessage, 'error').dismissOthers();
            });
        FlatPickerDateTime();
    },
    methods: {
        getInventories: function () {
            var self = this;
            axios.get(hostName + '/api/Inventories?warehouseId=' + self.warehouseId)
                .then(function (response) {
                    self.inventories = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        canSearch: function () {
            return this.warehouseId && this.inventoryId && this.fromDate && this.toDate;
        },
        search: function () {
            showLoader();
            var self = this;
            self.loading = true;
            self.reportData = null;
            self.selectedInventoryName = '';
            self.colSpan = "2";
            self.groupUpdatesByTypeValue = '';

            self.handelSelectedSensors();
            axios.get(hostName + '/Reports/InventoryReport/TemperatureAndHumidityInventoryHistoryReport', {
                params: {
                    inventoryId: self.inventoryId,
                    sensorsSN: self.sensorsSN.toString(),
                    fromDate: self.fromDate,
                    toDate: self.toDate,
                    groupUpdatesByType: self.groupUpdatesByType,
                    groupUpdatesValue: self.groupUpdatesValue
                }
            }).then(function (response) {
                //console.log(response.data);
                self.reportData = response.data;
                self.selectedInventoryName = self.inventories.filter(x => x.value == self.inventoryId)[0].text;

                self.colSpan = self.groupUpdatesByType ? "1" : "2";
                self.groupUpdatesByTypeValue = self.groupUpdatesByType;

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
        formatDateTime: function (date) {
            return moment(date).format("YYYY/MM/DD hh:mm A");
        },
        minTemperatureAndHumidityText: function (index) {
            var self = this;
            var temperatureList = self.reportData[index].historyRecords.map(x => {
                return x.temperature;
            });

            var humidityList = self.reportData[index].historyRecords.map(x => {
                return x.humidity;
            });

            var minTemperature = Math.min.apply(Math, temperatureList);
            var minHumidity = Math.min.apply(Math, humidityList);

            var text = "<small>&nbsp;[" + MinTemperatureText + " : " + minTemperature + "] [" + MinHumidityText + " : " + minHumidity + "]</span>";
            return text;
        },
        maxTemperatureAndHumidityText: function (index) {
            var self = this;
            var temperatureList = self.reportData[index].historyRecords.map(x => {
                return x.temperature;
            });

            var humidityList = self.reportData[index].historyRecords.map(x => {
                return x.humidity;
            });

            var maxTemperature = Math.max.apply(Math, temperatureList);
            var maxHumidity = Math.max.apply(Math, humidityList);

            var text = "<small>&nbsp;[" + MaxTemperatureText + " : " + maxTemperature + "] [" + MaxHumidityText + " : " + maxHumidity + "]</span>";
            return text;
        },
        exportToPDF: function () {
            var self = this;
            self.handelSelectedSensors();
            inventoryId = $("#InventoriesSelect").val();
            axios.get(hostName + '/Reports/InventoryReport/PrintPDF/' + self.inventoryId, {
                params: {
                    sensorsSN: self.sensorsSN.toString(),
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
                link.setAttribute('download', self.selectedInventoryName + '-InventoryReport-' + moment().format('YYYY-MM-DD') + '.pdf');
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
        },
        getSelectSensors: function () {
            var self = this;
            if (self.inventoryId > 0) {
                self.getSensors(self.inventoryId).then(function (response) {
                    var defaultSelectedValue = { text: all, value: 0 };
                    self.optionSensors.push(defaultSelectedValue);
                    self.selectedSensors.push(defaultSelectedValue);
                    for (var i = 0; i < response.data.length; i++) {
                        self.optionSensors.push({ text: response.data[i].text, value: response.data[i].value });
                        self.optionSensors.map(function (x) {
                            return x.sensorSelectLabel = x.text;
                        });
                    }

                }).catch(function (error) {
                    alertify.notify(errorMessage, 'error').dismissOthers();
                });
            }
        },
        sensorSelected: function (obj) {
            if (obj.length > 1) {
                for (var i = 0; i < obj.length; i ++)
                    if (obj[i].value == 0) {
                        obj.splice(i, 1);
                }
            }
        },
        getSensors: async function (inventoryId) {
            return await axios.get(hostName + "/api/InventorySensors?inventoryId=" + inventoryId);
        },
        handelSelectedSensors: function () {
            var self = this;
            self.sensorsSN = [];
            if (self.selectedSensors.length > 0) {
                for (var i = 0; i < self.selectedSensors.length; i++) {
                    if (self.selectedSensors[i].value == 0) {
                        self.selectedSensors.splice(i, 1);
                        break;
                    }
                    self.sensorsSN.push(self.selectedSensors[i].value);
                }
            }
        }
    }
});