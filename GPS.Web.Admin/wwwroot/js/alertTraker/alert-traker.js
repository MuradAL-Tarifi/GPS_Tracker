var PageSize = 25;
var PageNumber = 1;
var warehouseId;
var inventoryId;
var serial;
var id;
var SearchString;
$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    warehouseId = urlParams.get('warehouseId');
    if (!warehouseId) {
        warehouseId = '';
    }
    inventoryId = urlParams.get('inventoryId');
    if (!inventoryId) {
        inventoryId = '';
    }

    serial = urlParams.get('serial');
    if (!serial) {
        serial = '';
    }
    SearchString = urlParams.get('search');
    if (!SearchString) {
        SearchString = '';
    }
    $('#searchInput').val(SearchString);
    PageSize = urlParams.get('show');
    if (!PageSize) {
        PageSize = 25;
    }
    PageNumber = urlParams.get('page');
    if (!PageNumber) {
        PageNumber = 1;
    }

    InitFootable();
    inventoryId = '';
    warehouseId = '';
    serial = '';
    id = '';
    $("#WarehousSelectId").on("change", function () {
        warehouseId = $(this).val();
        $.ajax({
            type: "Get",
            url: hostName + "/Inventory/GetInventories?WarehouseId=" + $(this).val(),  //remember change the controller to your owns.  
            success: function (data) {
                debugger;
                var items = data;
                var s = '';
                if (items.length > 0) {
                    s += '<option value="0">' + all + '</option>';
                }
                $.each(items, function (index, val) {
                    debugger;
                    s += '<option value="' + val.value + '">' + val.text + '</option>';
                });
                $("#InventorySelectId").html(s);
                $('#InventorySelectId').trigger('change');
            },
            error: function (response) {
            }
        });
    });
    $("#InventorySelectId").on("change", function () {
        inventoryId = $(this).val();

        $.ajax({
            type: "Get",
            url: hostName + "/Sensors/GetInventorySensorsByInventoryId?InventoryId=" + $(this).val(),  //remember change the controller to your owns.  
            success: function (data) {
                debugger;
                var items = data;
                var s = '';
                if (items.length > 0) {
                    s += '<option value="0">' + all + '</option>';
                }
                $.each(items, function (index, val) {
                    debugger;
                    s += '<option value="' + val.value + '">' + val.text + '</option>';
                });
                $("#selectedSensorsId").html(s);
                $('#selectedSensorsId').trigger('change');
            },
            error: function (response) {
            }
        });
    });
    $("#searchbtn").on("click", function () {
        PageSize = 25;
        PageNumber = 1;
        warehouseId = $("#WarehousSelectId").val();
        inventoryId = $("#InventorySelectId").val();
        serial = $('#selectedSensorsId').val();
        var text = $('#searchInput').val().trim();
        SearchString = text;
        $.ajax({
            type: "Get",
            url: hostName + "/AlertTraker?warehouseId=" + warehouseId + "&inventoryId=" + inventoryId + "&serial=" + serial + "&search=" + SearchString + "&page=" + PageNumber + "&show=" + PageSize,  //remember change the controller to your owns.  
            success: function (result) {
                $('#PagedDataDiv').html(result);
                InitFootable();
            },
            error: function (response) {
            }
        });
    });
});
function InitFootable() {
    FooTable.init("#tbAlertBySensor");
}
function GetData() {
    PageNumber = 1;
    warehouseId = $("#WarehousSelectId").val();
    inventoryId = $("#InventorySelectId").val();
    serial = $('#selectedSensorsId').val();
    var text = $('#searchInput').val().trim();
    SearchString = text;
    $.ajax({
        type: "Get",
        url: hostName + "/AlertTraker?warehouseId=" + warehouseId + "&inventoryId=" + inventoryId + "&serial=" + serial + "&search=" + SearchString + "&page=" + PageNumber + "&show=" + PageSize,  //remember change the controller to your owns.  
        success: function (result) {
            $('#PagedDataDiv').html(result);
            InitFootable();
        },
        error: function (response) {
        }
    });
}
function createAlertBySensor(pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/AlertTraker?warehouseId=' + warehouseId + "&inventoryId=" + inventoryId + "&serial=" + serial + "&search=" + SearchString + "&page=" + pageNumber + "&show=" + PageSize);
    window.location.href = hostName + '/AlertTraker/Create?id=' + id + '&returnURL=' + returnURL;
}

function editAlertBySensor(id, pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/AlertTraker?warehouseId=' + warehouseId + "&inventoryId=" + inventoryId + "&serial=" + serial + "&search=" + SearchString + "&page=" + pageNumber + "&show=" + PageSize);
    window.location.href = hostName + '/AlertTraker/Create?id=' + id + '&returnURL=' + returnURL;
}
function PagedListSuccess() {
    InitFootable();
}
function ChangeSize(size) {
    debugger;
    PageSize = size;
    GetData();
}
function ConfirmDelete(Id, pageNumber) {
    SwalConfirm("warning", text, "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            var returnURL = hostName + '/AlertTraker?warehouseId=' + warehouseId + "&inventoryId=" + inventoryId + "&serial=" + serial + "&search=" + SearchString + "&page=" + pageNumber + "&show=" + PageSize;
            $("#returnURL").val(returnURL);
            $("#ItemId").val(Id);
            $("#DeleteForm").submit();
        }
    });
}