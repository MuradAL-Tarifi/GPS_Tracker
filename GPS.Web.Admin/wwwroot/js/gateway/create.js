$(document).ready(function () {
    $('[data-mask]').inputmask();
    FlatPickerDate();

    $("#Name").on("change", function (e) {
        $.get(hostName + "/Gateway/ValidateName", { Name: $("#Name").val().trim() }).done(function (exists) {
            exists ? $('#uniqueNameValidation').removeClass("d-none") : $('#uniqueNameValidation').addClass("d-none");
        });
    });

    $("#IMEI").on("change", function (e) {
        $.get(hostName + "/Gateway/ValidateIMEI", { IMEI: $("#IMEI").val().trim() }).done(function (exists) {
            exists ? $('#uniqueIMEIValidation').removeClass("d-none") : $('#uniqueIMEIValidation').addClass("d-none");
        });
    });

    $("#savebtn").click(function () {
        // validate
        var validator = $("#CreateForm").validate();
        if ($("#CreateForm").valid() && IsValid()) {
            $("#CreateForm").submit();
        }
        else {
            validator.focusInvalid();
        }
    });
});

function IsValid() {
    return $('#uniqueNameValidation').hasClass("d-none");
}