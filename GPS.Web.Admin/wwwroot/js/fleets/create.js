$(document).ready(function () {
    $('[data-mask]').inputmask();

    $("#FleetName").on("change", function (e) {
        $.get(hostName +"/Fleets/ValidateName", { AgentId: $("#AgentIdSelect").val(), Name: $("#FleetName").val().trim() }).done(function (exists) {
            exists ? $('#uniqueNameValidation').removeClass("d-none") : $('#uniqueNameValidation').addClass("d-none");
        });
    });

    $("#FleetNameEn").on("change", function (e) {
        $.get(hostName +"/Fleets/ValidateNameEn", { AgentId: $("#AgentIdSelect").val(), NameEn: $("#FleetNameEn").val().trim() }).done(function (exists) {
            exists ? $('#uniqueNameEnValidation').removeClass("d-none") : $('#uniqueNameEnValidation').addClass("d-none");
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
    return $('#uniqueNameValidation').hasClass("d-none") && $('#uniqueNameEnValidation').hasClass("d-none");
}