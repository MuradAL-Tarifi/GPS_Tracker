$(document).ready(function () {
    var hub = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();

    //Client Call
    hub.on("Notification", function (notification) {

    });

    hub.on("DisplayMessage", function (type, head, body) {
        Swal.mixin({
            toast: false,
            showConfirmButton: false,
            timer: 3000
        }).fire({
            type: type,
            title: head,
            text: body
        });
    });

    hub.start().then(function () {
        console.log("Hub Connected!");
    }).catch(function (err) {
        return console.error(err.toString());
    });
});