var fleetId = 0;


$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    FlatPickerDate();
    $("#AgentsSelect").on("change", function () {
        // get fleets
        var agentId = $(this).val();
        if (agentId > 0) {
            $.get(hostName +'/Users/GetFleets', { AgentId: agentId }).done(function (data) {
                var items = JSON.parse(data);
                var options = '';
                if (items.length > 0) {
                    options += '<option value="0">' + all + '</option>';
                }
                $.each(items, function (index, val) {
                    options += '<option value="' + val.Value + '">' + val.Text + '</option>';
                });
                $("#FleetsSelect").html(options);
                $('#FleetsSelect').trigger('change');
            });
        }
        else {
            // empty fleets select
            $("#FleetsSelect").html("");
            $('#FleetsSelect').trigger('change');
        }
    });

    $("#FleetsSelect").on("change", function () {
        fleetId = $(this).val();
        if (fleetId) {
            if (fleetId <= 0) {
                fleetId = null;
            } else {
                $.get(hostName + '/Users/GetWarehousesAndInventories', { fleetId: fleetId }).done(function (data) {
                    WarehousesInventoriesJsTree(JSON.parse(data));
                });
            }
            // get warehouses
            getWarehouses(fleetId);
        }
        else {
            // empty Warehouses Select
            $("#WarehousesSelect").html("");
            $('#WarehousesSelect').trigger('change');
        }
    });

    setTimeout(() => $("#FleetsSelect").trigger('change'), 500);

    $("#UserName").on("change", function (e) {
        $.get(hostName +"/Users/ValidateUsername", { Username: $("#UserName").val().trim() }).done(function (exists) {
            exists ? $('#uniqueUsernameValidation').removeClass("d-none") : $('#uniqueUsernameValidation').addClass("d-none");
        });
    });

    $("#Email").on("change", function (e) {
        $.get(hostName +"/Users/ValidateEmail", { Email: $("#Email").val().trim() }).done(function (exists) {
            exists ? $('#uniqueEmailValidation').removeClass("d-none") : $('#uniqueEmailValidation').addClass("d-none");
        });
    });

    $("#savebtn").click(function () {
        // validate
        var validator = $("#CreateForm").validate();
        if (IsValid() && $("#CreateForm").valid()) {
            $("#CreateForm").submit();
        }
        else {
            validator.focusInvalid();
        }
    });

    $("#roles-select").on('change', function () {
        if (this.value == "agent") {
            $('.agent-content').show();
        }
        else {
            $('.agent-content').hide();
            $("#inventoriesIds").val("");
        }
    });
});

function IsValid() {
    var valid = $('#uniqueUsernameValidation').hasClass("d-none") && $('#uniqueEmailValidation').hasClass("d-none");
    return valid;
}



function getWarehouses(fleetId) {
    $.get(hostName + "/Users/GetWarehouses", { FleetId: fleetId}).done(function (data) {
        var items = JSON.parse(data);
        var options = '';
        if (items.length > 0) {
            options += '<option value="0">' + all + '</option>';
        }
        $.each(items, function (index, val) {
            options += '<option value="' + val.Value + '">' + val.Text + '</option>';
        });
        $("#WarehousesSelect").html(options);
        $('#WarehousesSelect').trigger('change');
    });
}



