$(document).ajaxStart(function () { Pace.restart(); });

var app = new Vue({
    el: '#app',
    data: {
        timer: '',
        fleetData: null,
        warehouseData: null,
        searchText: "",
        hostName: hostName,
        speedFactor: 60,
        updating: true,
        isEnglish: isEnglish,
        //avgTemperature: 0,
        //avgHumidity: 0
    },
    created: function () {
        alertify.set('notifier', 'position', 'bottom-center');
    },
    mounted: function () {
        var self = this;

        setTimeout(function () {
            self.GetWarehouseData();
            self.GetFleetData();
        }, 500);

        self.timerUpdates();
    },
    methods: {
        GetWarehouseData: function () {
            var self = this;
            axios.get(hostName + '/Reports/Warehouse/OnlineHistory')
                .then(function (response) {
                    self.warehouseData = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        formatDatetime: function (date) {
            return moment(date).format("YYYY/MM/DD hh:mm A");
        },
        formatDateOnly: function (date) {
            return moment(date).format("YYYY/MM/DD");
        },
        stopUpdates: function () {
            var self = this;
            clearInterval(self.timer);
            self.updating = false;
        },
        resumeUpdates: function () {
            var self = this;
            self.timerUpdates();
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
                self.timerUpdates();
            }
        },
        timerUpdates: function () {
            var self = this;
            self.timer = setInterval(function () {
                self.GetWarehouseData();
                self.GetFleetData();
            }, self.speedFactor * 1000);
        }
    },
    //computed: {
    //    filteredList() {
    //        if (!this.searchText) {
    //            return this.warehouseData;
    //        }

    //        var filtered = [];
    //        var data = JSON.parse(JSON.stringify(this.warehouseData));
    //        for (var i = 0; i < data.length; i++) {
    //            var warehouse = data[i];
    //            warehouse.inventories = warehouse.inventories.filter(d => {
    //                return (
    //                    this.searchText != '' && (d.name.includes(this.searchText))
    //                );
    //            });

    //            if (warehouse.inventories.length > 0) {
    //                filtered.push(warehouse);
    //            }
    //        }
    //        return filtered;
    //    },
    //},
    beforeDestroy() {
        clearInterval(this.timer);
    }
});