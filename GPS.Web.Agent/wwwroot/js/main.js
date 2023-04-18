function setCookie(cookieName, cookieValue) {
    document.cookie = cookieName + "=" + cookieValue + ";path=/";
}
function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function removeCookie(cookieName) {
    document.cookie = cookieName + "= ; expires = Thu, 01 Jan 1970 00:00:00 GMT;path=/"
}



$(document).ready(function () {
    $.get(hostName + '/api/GetSystemSettings').done(function (response) {
        var data = JSON.parse(response);
        $("#companyName,#pageTitle,.brand-text").text(" " + data.CompanyName);
        if (data.LogoFileBase64.length > 0) {
            var imgSrc = `data:image/png;base64,${data.LogoFileBase64}`;
            $('.companyLogo').attr('src', imgSrc);
        }
    });

    var path = window.location.pathname.toLowerCase();
    var activeClass = "active";
    if (path.includes("reports") && path.includes("inventorysensor")) {
        //$("#reports > .nav-item").addClass("open");
        $("#InventorySensorLi").addClass(activeClass);
    } else if (path.includes("reports") && path.includes("sensor")) {
        //$("#reports > .nav-item").addClass("open");
        $("#SensorReportLi").addClass(activeClass);
    }
    else if (path.includes("reports") && path.includes("warehouseinventorysensor")) {
        //$("#reports > .nav-item").addClass("open");
        $("#WarehouseInventorySensorLi").addClass(activeClass);
    }
    else if (path.includes("reports") && path.includes("warehouse")) {
        $("#WarehouseLi").addClass(activeClass);
    }
    else if (path.includes("customalert")) {
        $("#CustomAlertLi").addClass(activeClass);
    }
    else if (path.includes("reports") && path.includes("inventory")) {
        //$("#reports > .nav-item").addClass("open");
        $("#InventoryLi").addClass(activeClass);
    }
    else if (path.includes("reports") && path.includes("averagedailytemperatureandhumidity")) {
        //$("#reports > .nav-item").addClass("open");
        $("#AverageDailyTemperatureAndHumidityReportLi").addClass(activeClass);
    }
    else if (path.includes("scheduling")) {
        //$("#reports > .nav-item").addClass("open");
        $("#ScheduleReportsLi").addClass(activeClass);
    }
    else if (path.includes("users")) {
        $("#UsersLi").addClass(activeClass);
        $("#SystemManagementLi").addClass("open");
    } else if (path.includes("companysetting")) {
        $("#CompanySettingsLi").addClass(activeClass);
    }
});



function hideLoader() {
    $('.app-content .content-overlay').css({
        'z-index': '-1',
        'opacity': '0'
    });
}
function showLoader() {
    $('.app-content .content-overlay').css({
        'z-index': '999',
        'opacity': '1'
    });
}


function FlatPickerDate() {
    var flatPickr = $('.flatPickr-date');
    if (flatPickr.length) {
        flatPickr.flatpickr({
            dateFormat: 'Y-m-d',
            defaultDate: 'today',
            onReady: function (selectedDates, dateStr, instance) {
                if (instance.isMobile) {
                    $(instance.mobileInput).attr('step', null);
                }
            }
        });
    }
    $('.flatpickr-basic').flatpickr();
}

function FlatPickerDateTime() {
   var dateTimePickr = $('.flatpickr-date-time');
    // Date & TIme
    if (dateTimePickr.length) {
        dateTimePickr.flatpickr({
            enableTime: true,
            dateFormat: 'Y-m-d H:i',
        });
    }
}

function ToTwoDecimalPlaces(val) {
    if (Math.round(val) != val) {
        val = val.toFixed(2)
    }
    return val;
}

function WarehousesInventoriesJsTree() {
    var warehousesIds = [];
    $("#jstree-warehouses-inventories").jstree({
        core: {
            data: WarehousesInventories
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
                icon: 'fa fa fa-microchip text-info'
            },
        }
    });
    var warehouses = JSON.parse(JSON.stringify(WarehousesInventories));
    for (var i = 0; i < warehouses.length; i++) {
        warehousesIds.push(warehouses[i].id);
    }

    //on change js tree
    $('#jstree-warehouses-inventories').on('changed.jstree', function (e, data) {
        var ids = [];
        var i, j;
        for (i = 0, j = data.selected.length; i < j; i++) {
            var id = data.instance.get_node(data.selected[i]).id;
            ids.push(id);
        }
        for (var x = 0; x < warehousesIds.length; x++) {
            const index = ids.indexOf(warehousesIds[x].toString());
            if (index > 0) {
                ids.splice(index, 1);
            }
        }
        $("#inventoriesIds").val(ids.toString());
    });
}