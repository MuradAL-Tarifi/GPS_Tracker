
$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {

    FlatPickerDate();
    WarehousesInventoriesJsTree();
    GetWarehouses();

    $("#UserName").on("change", function (e) {
        $.get(hostName +"/Users/ValidateUsername", { Username: $("#UserName").val().trim() }).done(function (exists) {
            exists ? $('#uniqueUsernameValidation').removeClass("d-none") : $('#uniqueUsernameValidation').addClass("d-none");
        });
    });

    $(".flatPickr-date").change(function () {
        $.get(hostName + "/Users/ValidateExpirationDate", { ExpirationDate: this.value }).done(function (response) {
            $('#expirationDateExceed').text(expirationDateExceed.replace("-", response));
            response.length > 0 ? $('#expirationDateExceed').removeClass("d-none") : $('#expirationDateExceed').addClass("d-none");
        });
    });

    $("#Email").on("change", function (e) {
        $.get(hostName + "/Users/ValidateEmail", { Email: $("#Email").val().trim() }).done(function (exists) {
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

});

function IsValid() {
    var valid = $('#uniqueUsernameValidation').hasClass("d-none") &&
        $('#uniqueEmailValidation').hasClass("d-none") &&
        $('#expirationDateExceed').hasClass("d-none");
    return valid;
}



function GetWarehouses() {
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