$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    FlatPickerDate();
    WarehousesInventoriesJsTree();
    GetWarehouses();

    $("#UserName").on("change", function (e) {
        if ($("#UserName").val().trim() == currentUsername) {
            if (!$('#uniqueUsernameValidation').hasClass("d-none")) {
                $('#uniqueUsernameValidation').addClass("d-none");
            }
        }
        else {
            $.get(hostName + "/Users/ValidateUsername", { Username: $("#UserName").val().trim() }).done(function (exists) {
                exists ? $('#uniqueUsernameValidation').removeClass("d-none") : $('#uniqueUsernameValidation').addClass("d-none");
            });
        }
    });

    $(".flatPickr-date").change(function () {
        $.get(hostName + "/Users/ValidateExpirationDate", { ExpirationDate: this.value }).done(function (response) {
            $('#expirationDateExceed').text(expirationDateExceed.replace("-", response));
            response.length > 0 ? $('#expirationDateExceed').removeClass("d-none") : $('#expirationDateExceed').addClass("d-none");
        });
    });

    $("#Email").on("change", function (e) {
        if ($("#Email").val().trim() == currentEmail) {
            if (!$('#uniqueEmailValidation').hasClass("d-none")) {
                $('#uniqueEmailValidation').addClass("d-none");
            }
        }
        else {
            $.get(hostName + "/Users/ValidateEmail", { Email: $("#Email").val().trim() }).done(function (exists) {
                exists ? $('#uniqueEmailValidation').removeClass("d-none") : $('#uniqueEmailValidation').addClass("d-none");
            });
        }
    });

    $("#updatebtn").click(function () {
        // validate
        var validator = $("#UpdateForm").validate();
        if (IsValid() && $("#UpdateForm").valid()) {
            SwalConfirm("question", text, "", yes, cancel, function (result) {
                if (result) {
                    $("#UpdateForm").submit();
                }
            });
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
    $.get(hostName + "/Users/GetWarehouses", { FleetId: fleetId }).done(function (data) {
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