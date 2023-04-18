$(document).ready(function () {
    var currentName = $("#FleetName").val();
    var currentNameEn = $("#FleetNameEn").val();

    $('[data-mask]').inputmask();

    $("#FleetName").on("change", function (e) {
        if ($("#FleetName").val().trim() == currentName) {
            if (!$('#uniqueNameValidation').hasClass("d-none")) {
                $('#uniqueNameValidation').addClass("d-none");
            }
        }
        else {
            $.get(hostName +"/Fleets/ValidateName", { AgentId: $("#AgentIdSelect").val(), Name: $("#FleetName").val().trim() }).done(function (exists) {
                exists ? $('#uniqueNameValidation').removeClass("d-none") : $('#uniqueNameValidation').addClass("d-none");
            });
        }
    });

    $("#FleetNameEn").on("change", function (e) {
        if ($("#FleetNameEn").val().trim() == currentNameEn) {
            if (!$('#uniqueNameEnValidation').hasClass("d-none")) {
                $('#uniqueNameEnValidation').addClass("d-none");
            }
        }
        else {
            $.get(hostName +"/Fleets/ValidateNameEn", { AgentId: $("#AgentIdSelect").val(), NameEn: $("#FleetNameEn").val().trim() }).done(function (exists) {
                exists ? $('#uniqueNameEnValidation').removeClass("d-none") : $('#uniqueNameEnValidation').addClass("d-none");
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
    return $('#uniqueNameValidation').hasClass("d-none") && $('#uniqueNameEnValidation').hasClass("d-none");
}