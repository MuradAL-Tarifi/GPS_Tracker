$(document).ajaxStart(function () { Pace.restart(); });

var app = new Vue({
    el: '#app',
    data: {
        timer: '',
        fleetData: null,
        selectedInventory: null,
        searchText: "",
        hostName: hostName,
        speedFactor: 15,
        updating: true,
        isEnglish: isEnglish,
        avgTemperature: 0,
        avgHumidity: 0,
        isOnlyAverageTemp: false,
        selectedInventories: null,
        warehouseName: '',
        inventoryIds: []
    },
    created: function () {
        alertify.set('notifier', 'position', 'bottom-center');
    },
    mounted: function () {
        showLoader();
        var self = this;
        setTimeout(function () { self.GetInitialData(); }, 500);
        self.timer = setInterval(self.fetchNewData, self.speedFactor * 1000);
        setTimeout(function () {
            hideLoader();
        }, 1400);
    },
    methods: {
        GetInitialData: function () {
            var self = this;
            axios.get(hostName + '/Reports/OnlineHistory')
                .then(function (response) {
                    if (response.data.length > 0) {
                        self.fleetData = response.data;
                        self.selectInventory(self.fleetData[0].inventories[0], null, self.fleetData[0].name);
                    }
                })
                .catch(function (error) {
                    //alertify.notify(ErrorMessage, 'error').dismissOthers();
                    console.log("1" +error);
                });
        },
        fetchNewData: function () {
            var self = this;
            if (!self.selectedInventories) {
                axios.get(hostName + '/Reports/InventoryOnlineHistory?inventoryId=' + self.selectedInventory.id)
                    .then(function (response) {
                        //console.log(response.data);

                        self.selectedInventory.inventorySensors = response.data;
                        var acceptedList = self.selectedInventory.inventorySensors.filter(x => x.isAccepted);

                        self.avgTemperature = 0;
                        self.avgHumidity = 0;

                        if (acceptedList.length > 0) {
                            $.each(acceptedList, function (index, sensor) {
                                self.avgTemperature += sensor.temperature;
                                self.avgHumidity += sensor.humidity;
                            });
                        }

                        var lsValidTemperatureSensors = acceptedList.filter(x => x.isTemperatureAccepted);
                        var lsValidHumiditySensors = acceptedList.filter(x => x.isHumidityAccepted);

                        self.isOnlyAverageTemp = ((lsValidTemperatureSensors.length == acceptedList.length) && lsValidHumiditySensors.length == 0) || self.avgHumidity == 0;
                        if (self.avgTemperature > 0 || (self.avgTemperature * -1 > 0) && self.avgTemperature != 0) {
                            self.avgTemperature = parseFloat(self.avgTemperature / lsValidTemperatureSensors.length).toFixed(2);
                        }
                        if (self.avgHumidity > 0 | (self.avgTemperature * -1 > 0) && self.avgHumidity != 0) {
                            self.avgHumidity = parseFloat(self.avgHumidity / lsValidHumiditySensors.length).toFixed(2);
                        }
                    })
                    .catch(function (error) {
                        //alertify.notify(ErrorMessage, 'error').dismissOthers();
                        console.log("2" + error);
                    });
            } else {
                if (self.inventoryIds.length > 0) {
                    axios.get(hostName + '/Reports/InventoriesOnlineHistory?inventoryIds=' + self.inventoryIds.toString())
                        .then(function (response) {
                            //console.log(response.data);
                            for (var i = 0; i < self.selectedInventories.length; i++) {
                                var onlineInventory = response.data.filter(x => x.inventoryId == self.selectedInventories[i].id);
                                self.selectedInventories[i].inventorySensors = onlineInventory[0].lsInventorySensorTemperatureModel;
                                //console.log(self.selectedInventories[i].inventorySensors);
                            }

                        })
                        .catch(function (error) {
                            //alertify.notify(ErrorMessage, 'error').dismissOthers();
                            console.log("2" + error);
                        });
                }
            }
        },
        selectInventory: function (inventory, event, warehouseName) {
            var self = this;
            self.selectedInventories = null;
            self.warehouseName = warehouseName;
            var setPointer = false;

            if (self.selectedInventory) {
                self.selectedInventory.inventorySensors = [];
            }

            if (event && event.target.tagName == "INPUT" && !event.target.checked) {
                self.UnselectCurrentInventory();
            }
            else {
                if (!inventory.selected || inventory.selected && self.selectedInventory != inventory) {
                    setPointer = true;
                }

                if (setPointer) {
                    inventory.selected = true;
                    self.selectedInventory = inventory;
                    self.resetTimer();
                }
            }

            self.fetchNewData();
        },
        selectInventories: function (event, warehouse) {
            var self = this;
            if (warehouse) {
                self.warehouseName = warehouse.name;
                self.selectedInventories = warehouse.inventories;
                //self.UnselectCurrentInventory();
                self.inventoryIds = [];

                for (var x = 0; x < self.selectedInventories.length; x++) {
                    self.inventoryIds.push(self.selectedInventories[x].id);
                }
                self.resetTimer();

                self.fetchNewData();
            }
        },
        UnselectCurrentInventory() {
            if (this.selectedInventory) {
                this.selectedInventory = null;
            }
        },
        formatDatetime: function (date) {
            return moment(date).format("YYYY/MM/DD hh:mm A");
        },
        //viewHistory: function (sensorSerial) {
        //    var self = this;

        //    var fromDate = moment().subtract(1, "days").format("YYYY/MM/DD hh:mm A");
        //    var toDate = moment().format("YYYY/MM/DD hh:mm A");
        //    var url = hostName + '/Reports/InventorySensor?page=1&show=100&fromDate=' + fromDate +
        //        '&toDate=' + toDate +
        //        '&warehouseId=' + self.selectedInventory.warehouseId +
        //        '&inventoryId=' + self.selectedInventory.id +
        //        '&sensorSerial=' + sensorSerial;
        //    window.location.href = url;
        //    //window.open(url, "_blank", 'noopener');
        //},
        historyUrl: function (sensorSerial) {
            var self = this;

            var fromDate = moment().subtract(1, "days").format("YYYY/MM/DD hh:mm A");
            var toDate = moment().format("YYYY/MM/DD hh:mm A");
            var url = hostName + '/Reports/InventorySensor?page=1&show=100&fromDate=' + fromDate +
                '&toDate=' + toDate +
                '&warehouseId=' + self.selectedInventory.warehouseId +
                '&inventoryId=' + self.selectedInventory.id +
                '&sensorSerial=' + sensorSerial;
            return url;
        },
        stopUpdates: function () {
            var self = this;
            clearInterval(self.timer);
            self.updating = false;
        },
        resumeUpdates: function () {
            var self = this;
            self.timer = setInterval(self.fetchNewData, self.speedFactor * 1000);
            self.updating = true;
        },
        SpeedUp: function () {
            var self = this;
            if (self.speedFactor > 5) {
                self.speedFactor = self.speedFactor - 1;
                self.resetTimer();
            }
        },
        ResetSpeed: function () {
            var self = this;
            self.speedFactor = 15;
            self.resetTimer();
        },
        SpeedDown: function () {
            var self = this;
            self.speedFactor = this.speedFactor + 1;
            self.resetTimer();
        },
        resetTimer() {
            var self = this;
            if (self.updating) {
                clearInterval(self.timer);
                self.timer = setInterval(self.fetchNewData, self.speedFactor * 1000);
            }
        },
    },
    computed: {
        filteredList() {
            if (!this.searchText) {
                return this.fleetData;
            }

            let filtered = [];
            var data = JSON.parse(JSON.stringify(this.fleetData));
            if (data.some(x => x.name.toLowerCase().includes(this.searchText.toLowerCase().trim())) && this.searchText != '') {
                filtered = data.filter(d => {
                    return (d.name.toLowerCase().includes(this.searchText.toLowerCase().trim()));
                });
                return filtered;
            }
            for (var i = 0; i < data.length; i++) {
                var warehouse = data[i];
                warehouse.inventories = warehouse.inventories.filter(d => {
                    return (
                        this.searchText != '' && (d.name.toLowerCase().includes(this.searchText.toLowerCase().trim()))
                    );
                });

                if (warehouse.inventories.length > 0) {
                    filtered.push(warehouse);
                }
            }
            
            return filtered;
        },
    },
    beforeDestroy() {
        clearInterval(this.timer);
    }
});