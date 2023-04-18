
//$(document).ajaxStart(function () { Pace.restart(); });

var notification = new Vue({
    el: "#notifications",
    data:
    {
        numberOfNotifications: 0,
        speedFactor: 9,
        notifications:null
    },
    mounted: function () {
        var self = this;
        getNewNotifications();
        self.timer = setInterval(getNewNotifications(), self.speedFactor * 1000);
    },
    created: function () {
        var self = this;
        window.getNewNotifications = function () {
            $.ajax({
                url: hostName + "/api/Alerts",
                type: 'GET',
                success: function (response) {
                    //console.log(response);
                    if (response.isSuccess) {
                        if (response.data.length > 0) {
                            self.notifications = response.data;
                            self.numberOfNotifications = response.data[0].numberOfNewNotifications;
                        }
                    }
                },
                error: function (xhr, status, p3, p4) {
                    //alertify.notify(errorMessage, 'error').dismissOthers();
                    console.log("notifications error");
                }
            });
        }
    },
    methods: {
        viewSelectedAlert: function (warehouseId = null, inventoryId = null, sensorId = null, alertTypeLookupId = null, alertId = null) {
            if (warehouseId > 0 && inventoryId > 0 && sensorId > 0 && alertTypeLookupId > 0 && alertId > 0) {
                window.location.href = hostName + '/Reports/Alert?warehouseId=' + warehouseId + '&inventoryId=' + inventoryId + '&sensorId=' + sensorId + '&alertType=' + alertTypeLookupId + '&alertId=' + alertId;
            } else {
                window.location.href = hostName + '/Reports/Alert';
            }
        }
    }
});