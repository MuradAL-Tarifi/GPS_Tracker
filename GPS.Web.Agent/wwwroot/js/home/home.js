$(document).ajaxStart(function () { Pace.restart(); });

var app = new Vue({
    el: "#app",
    data:
    {
        agentDashboard: JSON.parse(JSON.stringify(agentDashboardViewModel)),
        sensorsCount:null,
    },
    mounted: function ()
    {
        
        if (this.sensorsCount == 0) {
            return;
        }
        let totalInstalledSenesor = this.agentDashboard.workingSensorsCount + this.agentDashboard.notWorkingSensorsCount;
        let calcPercentageOfWorkingSensors =
            ToTwoDecimalPlaces((this.agentDashboard.workingSensorsCount / totalInstalledSenesor) * 100);
        let calcPercentageOfNotWorkingSensors =
            ToTwoDecimalPlaces((this.agentDashboard.notWorkingSensorsCount / totalInstalledSenesor) * 100);
        // working sensors
        this.RenderWorkingNotWorkingSensorsChart(document.querySelector("#sensors-working-view-chart"), calcPercentageOfWorkingSensors);

        // not working sensors
        this.RenderWorkingNotWorkingSensorsChart(document.querySelector("#sensors-notWorking-view-chart"), calcPercentageOfNotWorkingSensors);

    },
    created: function () {
        this.sensorsCount = this.agentDashboard.workingSensorsCount + this.agentDashboard.notWorkingSensorsCount;
    },
    methods: {
        RenderWorkingNotWorkingSensorsChart: function (elementViewChart, calcPercentage) {
            let goalStrokeColor = elementViewChart.id == 'sensors-working-view-chart' ? '#51e5a8' : '#de242d';
            var sensorsViewChartOpition = {
                chart: {
                    height: 245,
                    type: 'radialBar',
                    sparkline: {
                        enabled: true
                    },
                    dropShadow: {
                        enabled: true,
                        blur: 3,
                        left: 1,
                        top: 1,
                        opacity: 0.1
                    }
                },
                colors: [goalStrokeColor],
                plotOptions: {
                    radialBar: {
                        offsetY: -10,
                        startAngle: -150,
                        endAngle: 150,
                        hollow: {
                            size: '77%'
                        },
                        track: {
                            background: '#ebe9f1',
                            strokeWidth: '50%'
                        },
                        dataLabels: {
                            name: {
                                show: false
                            },
                            value: {
                                color: '#5e5873',
                                fontSize: '2.86rem',
                                fontWeight: '600'
                            }
                        }
                    }
                },
                fill: {
                    type: 'gradient',
                    gradient: {
                        shade: 'dark',
                        type: 'horizontal',
                        shadeIntensity: 0.5,
                        gradientToColors: [elementViewChart.id == 'sensors-working-view-chart' ? window.colors.solid.success : window.colors.solid.danger],
                        inverseColors: true,
                        opacityFrom: 1,
                        opacityTo: 1,
                        stops: [0, 100]
                    }
                },
                series: [calcPercentage],
                stroke: {
                    lineCap: 'round'
                },
                grid: {
                    padding: {
                        bottom: 30
                    }
                }
            }

            var goalOverviewChart = new ApexCharts(elementViewChart, sensorsViewChartOpition);
            goalOverviewChart.render();
        }

    }
});





