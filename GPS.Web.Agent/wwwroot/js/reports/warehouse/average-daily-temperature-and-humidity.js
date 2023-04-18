
$(document).ajaxStart(function () { Pace.restart(); });


var app = new Vue({
    el: '#app',
    data: {
        warehouses: [],
        inventories: [],
        warehouseId: '',
        inventoryId: '',
        fromDate: null,
        toDate: null,
        lsGroupedChartData: null,
        currentDate: '',
    },
    created: function () {
        var self = this;
        alertify.set('notifier', 'position', 'bottom-center');

        var fromDate = moment();
        fromDate.hour(0);
        fromDate.minutes(0);
        fromDate.seconds(0);
        fromDate.milliseconds(0);

        self.fromDate = fromDate.format("YYYY/MM/DD hh:mm A");

        var toDate = fromDate.add(1, 'days');

        self.toDate = toDate.format("YYYY/MM/DD hh:mm A");

        self.currentDate = moment().format("YYYY/MM/DD");
    },
    mounted: function () {
        this.getWarehouses();
        FlatPickerDate();
    },
    watch: {
        fromDate: function (newValue, oldValue) {
            var self = this;
            self.toDate = moment(self.fromDate).add(1, "days").format("YYYY/MM/DD hh:mm A");
            self.currentDate = moment(self.fromDate).format("YYYY/MM/DD");
        }
    },
    methods: {
        checkDayFilter: function () {
            var self = this;
            var diff = moment(self.toDate).diff(moment(self.fromDate), 'days');
            if (diff > 1) {
                self.toDate = moment(self.fromDate).add(1, "days").format("YYYY/MM/DD hh:mm A");
            }
        },
        canSearch: function () {
            return this.warehouseId && this.inventoryId && this.fromDate && this.toDate;
        },
        getWarehouses: function () {
            var self = this;
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
            axios.get(hostName + '/api/Inventories?warehouseId=' + self.warehouseId)
                .then(function (response) {
                    self.inventories = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });
        },
        search: function () {
            var self = this;
            self.reportData = null;
            self.selectedInventoryName = '';

            axios.get(hostName + '/Reports/GetAverageDailyTemperatureAndHumidityReport', {
                params: {
                    inventoryId: self.inventoryId,
                    fromDate: self.fromDate,
                    toDate: self.toDate
                }
            }).then(function (response) {
                self.lsGroupedChartData = response.data;
            }).catch(function (error) {
                self.lsGroupedChartData = [];
                if (error.response.status != 404) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                }
            }).finally(function () {
                if (self.lsGroupedChartData && self.lsGroupedChartData.length > 0) {
                    self.generateChart();
                }
            });
        },
        generateChart: function () {
            var self = this;
            $.each(self.lsGroupedChartData, function (index, groupedChartData) {
                var temperatureData = [];
                var humidityData = [];
                $.each(groupedChartData.lsInventorySensorTemperatureAndHumidityChartHeaderInfo, function (index, item) {
                    temperatureData.push([new Date(item.hourText).getTime(), item.averageTemperature]);
                    humidityData.push([new Date(item.hourText).getTime(), item.averageHumidity]);

                    var temperature_data = {
                        data: temperatureData,
                        color: '#dc3545'
                    }

                    var humidity_data = {
                        data: humidityData,
                        color: '#3c8dbc'
                    }

                    $.plot('#temperature-humidity-chart-' + groupedChartData.sensorName , [temperature_data, humidity_data], {
                        grid: {
                            hoverable: true,
                            borderColor: '#f3f3f3',
                            borderWidth: 1,
                            tickColor: '#f3f3f3'
                        },
                        series: {
                            shadowSize: 0,
                            lines: {
                                show: true
                            },
                            points: {
                                show: true
                            }
                        },
                        lines: {
                            fill: false,
                            color: ['#3c8dbc', '#f56954']
                        },
                        yaxis: {
                            show: true,
                            axisLabel: "الرطوبة",
                            axisLabelUseCanvas: true,
                            axisLabelFontSizePixels: 12,
                            axisLabelFontFamily: 'Verdana, Arial',
                            axisLabelPadding: 3,
                            tickDecimals: 2,
                        },
                        xaxis: {
                            axisLabel: "الساعة",
                            axisLabelUseCanvas: true,
                            axisLabelFontSizePixels: 12,
                            axisLabelFontFamily: 'Verdana, Arial',
                            axisLabelPadding: 10,
                            mode: "time",
                            timeformat: "%I:%M <br>%P",
                            tickSize: [1, "hour"],
                            twelveHourClock: true,
                            min: new Date(self.fromDate).getTime(), // time right now - 24 hours ago in milliseonds
                            max: new Date(self.toDate).getTime(),
                            timezone: "browser" // switch to using local time on plot
                        }
                    })
                    //Initialize tooltip on hover
                    $(`<div class="tooltip-inner" id="temperature-humidity-chart-tooltip-${groupedChartData.sensorName}"></div>`).css({
                        position: 'absolute',
                        display: 'none',
                        opacity: 0.8,
                        "z-index": 9999
                    }).appendTo('body')
                    $('#temperature-humidity-chart-'+groupedChartData.sensorName).bind('plothover', function (event, pos, item) {
                        if (item) {

                            var x = item.datapoint[0],
                                y = item.datapoint[1]

                            var hour = new moment(x).format("hh:mm A");
                            if (item.seriesIndex == 1) {
                                $('#temperature-humidity-chart-tooltip-'+groupedChartData.sensorName).html(hour + ' : ' + y + "%")
                                    .css({
                                        top: item.pageY + 5,
                                        left: item.pageX + 5,
                                        "direction": "ltr"
                                    })
                                    .fadeIn(200)
                            }
                            else {
                                $('#temperature-humidity-chart-tooltip-'+groupedChartData.sensorName).html(hour + ' : ' + y + "°C")
                                    .css({
                                        top: item.pageY + 5,
                                        left: item.pageX + 5,
                                        "direction": "ltr"
                                    })
                                    .fadeIn(200)
                            }
                        } else {
                            $('#temperature-humidity-chart-tooltip-'+groupedChartData.sensorName).hide()
                        }
                    })
                });
            });
            if ($("html").hasClass("dark-layout")) {
                $('.flot-tick-label').css('color', 'white');
            }
            
        }
    }
});