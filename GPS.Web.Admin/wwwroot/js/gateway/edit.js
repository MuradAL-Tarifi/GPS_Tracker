$(document).ready(function () {
    var currentName = $("#Name").val();
    var currentNameEn = $("#NameEn").val();
    var currentIMEI = $("#IMEI").val();
    FlatPickerDate();
    $('[data-mask]').inputmask();

    $("#Name").on("change", function (e) {
        if ($("#Name").val().trim() == currentName) {
            if (!$('#uniqueNameValidation').hasClass("d-none")) {
                $('#uniqueNameValidation').addClass("d-none");
            }
        }
        else {
            $.get(hostName + "/Gateway/ValidateName", { Name: $("#Name").val().trim() }).done(function (exists) {
                exists ? $('#uniqueNameValidation').removeClass("d-none") : $('#uniqueNameValidation').addClass("d-none");
            });
        }
    });

    $("#IMEI").on("change", function (e) {
        if ($("#IMEI").val().trim() == currentIMEI) {
            if (!$('#uniqueIMEIValidation').hasClass("d-none")) {
                $('#uniqueIMEIValidation').addClass("d-none");
            }
        }
        else {
            $.get(hostName + "/Gateway/ValidateIMEI", { IMEI: $("#IMEI").val().trim() }).done(function (exists) {
                exists ? $('#uniqueIMEIValidation').removeClass("d-none") : $('#uniqueIMEIValidation').addClass("d-none");
            });
        }
    });

    $("#updatebtn").click(function () {
        // validate
        var validator = $("#UpdateForm").validate();
        if ($("#UpdateForm").valid() && IsValid()) {
            $("#UpdateForm").submit();
        }
        else {
            validator.focusInvalid();
        }
    });
});

function IsValid() {
    return $('#uniqueNameValidation').hasClass("d-none") && $('#uniqueIMEIValidation').hasClass("d-none");
}