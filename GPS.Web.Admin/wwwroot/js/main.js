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


//$(window).on('load', function () {
//    if (getCookie("dark-mode-layout").length > 0) {
//        $('.header-navbar .nav-link-style .feather').removeClass("feather-moon");
//        $('.header-navbar .nav-link-style .feather').addClass("feather-sun");
//    } else {
//        $('.header-navbar .nav-link-style .feather').removeClass("feather-sun");
//        $('.header-navbar .nav-link-style .feather').addClass("feather-moon");
//    }
//});

$(document).ready(function () {
    $.get(hostName + '/SystemSetting/GetSystemSettings').done(function (response) {
        var data = JSON.parse(response);
        $("#companyName,#pageTitle,.brand-text").text(" " + data.CompanyName);
        if (data.LogoFileBase64.length > 0) {
            var imgSrc = `data:image/png;base64,${data.LogoFileBase64}`;
            $('.companyLogo').attr('src', imgSrc);
        }
    });
    var activeClass = "active";
    var path = window.location.pathname.toLowerCase();
    //alert(path);

    if (path.includes("wasl") && !path.includes("editwasl") && !path.includes("createwasl")) {
        $("#WASLInquiriesLi").addClass("menu-open active");
        $("#WASLInquiriesLi a:first").addClass("active");
        $("#WASLInquiriesLi ul.treeview-menu").css("display", "block");
        if (path.includes("operatingcompanies")) {
            $("#WASLOperatingCompaniesLi a").addClass(activeClass);
            $("#WASLInquiriesLi").addClass("open");
        }
        else if (path.includes("warehouses")) {
            $("#WASLWarehousesLi a").addClass(activeClass);
            $("#WASLInquiriesLi").addClass("open");
        }
    }
    else if (path.includes("companies")) {
        $("#FleetsLi").addClass(activeClass);
    }
    else if (path.includes("groups")) {
        $("#GroupsLi").addClass(activeClass);
    }
    else if (path.includes("users")) {
        $("#UsersLi").addClass(activeClass);
        $("#SystemManagementLi").addClass("open");
    }
    else if (path.includes("operations")) {
        $("#OperationsLi").addClass(activeClass);
        $("#SystemManagementLi").addClass("open");
    }
    else if (path.includes("sensors")) {
        $("#SensorsLi").addClass(activeClass);
    }
    else if (path.includes("gateway")) {
        $("#GatewayLi").addClass(activeClass);
    }
    else if (path.includes("warehouse")) {
        $("#WarehouseLi").addClass(activeClass);
    }
    else if (path.includes("inventory")) {
        $("#InventoryLi").addClass(activeClass);
    }
    else if (path.includes("operations")) {
        $("#OperationsLi").addClass(activeClass);
    }
    else if (path.includes("systemsetting")) {
        $("#SystemSettingsLi").addClass(activeClass);
    }
    else {
        $("#DashboardLi").addClass(activeClass);
    }
});



function hideLoader() {
    $(".loading-loader").addClass('d-none');
}
function showLoader() {
    $(".loading-loader").removeClass('d-none');
}



function FlatPickerDate() {
    var flatPickr = $('.flatPickr-date');
    if (flatPickr.length) {
        flatPickr.flatpickr({
            dateFormat: 'Y-m-d',
            onReady: function (selectedDates, dateStr, instance) {
                if (instance.isMobile) {
                    $(instance.mobileInput).attr('step', null);
                }
            }
        });
    }
    $('.flatpickr-basic').flatpickr();
}

function ToTwoDecimalPlaces(val) {
    if (Math.round(val) != val) {
        val = val.toFixed(2)
    }
    return val;
}

function WarehousesInventoriesJsTree(WarehousesInventories = null) {
    if (WarehousesInventories != null) {
        var warehousesIds = [];
      $("#jstree-warehouses-inventories").jstree({
            core: {
                data: WarehousesInventories,
                check_callback: true,
            },
            plugins: ['types', 'checkbox', 'wholerow'],
            types: {
                default: {
                    icon: 'far fa-folder'
                }
            }
       });
        
        $("#jstree-warehouses-inventories").jstree(true).settings.core.data = WarehousesInventories;
        $("#jstree-warehouses-inventories").jstree(true).refresh();
        
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
    
}