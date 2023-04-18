$(document).ready(function () {
    $('[data-mask]').inputmask();

    $("#saveBtn").click(function () {
        Save();
    });

    $("#ActivityTypeSelect").change(function () {
        $("#SFDARow").css("display", "none");

        if (this.value == 'SFDA') {
            $("#SFDARow").css("display", "");
        }
    });

    if ($("#ActivityTypeSelect").val() == 'SFDA') {
        $("#SFDARow").css("display", "");
    }



    if ($("#fleetType").val() == 'company') {
        $(".companyRow").css("display", "");
        $(".individualRow").css("display", "none");
    }
    else {
        $(".individualRow").css("display", "");
        $(".companyRow").css("display", "none");
    }

    $('.disabled').attr('disabled', 'disabled');
});

function Save() {
    // validate
    var validator = $("#EditWaslForm").validate();
    if ($("#EditWaslForm").valid()) {
        SwalConfirm("question", text, "", yes, cancel, function (result) {
            if (result) {
                $("#EditWaslForm").submit();
            }
        });
    }
    else {
        validator.focusInvalid();
    }
}


function ConfirmLinkWithWasl(Id) {
    SwalConfirm("warning", "هل انت متأكد ؟", "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            axios({
                method: 'post',
                url: hostName + '/Fleets/LinkWithWasl/' + Id,
                headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
            })
                .then(function (response) {
                    if (!response.data.isSuccess) {
                        SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                    } else {
                        SwalAlertOk("success", "تم الربط بنجاح", "", function (result) {
                            setTimeout(function () {
                                window.location.reload();
                            }, 500);
                        });
                    }
                    hideLoader();
                })
                .catch(function (error) {
                    if (error.response.data.errorList) {
                        SwalAlert("error", "", error.response.data.errorList[0]);
                    }
                    else {
                        SwalAlert("error", "حدث خطأ في النظام", "");
                    }
                    hideLoader();
                });
        }
    });
}

function ConfirmUnlinkWithWasl(Id) {
    SwalConfirm("warning", "هل انت متأكد ؟", "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            axios({
                method: 'post',
                url: hostName + '/Fleets/UnlinkWithWasl/' + Id,
                headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
            })
                .then(function (response) {
                    if (!response.data.isSuccess) {
                        SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                    } else {
                        SwalAlertOk("success", "تم إلغاء الربط بنجاح", "", function (result) {
                            setTimeout(function () {
                                window.location.reload();
                            }, 500);
                        });
                    }
                    hideLoader();
                })
                .catch(function (error) {
                    if (error.response.data.errorList) {
                        if (error.response.data) {
                            SwalConfirm("warning", error.response.data.errorList[0], "هل تريد فك الربط لكافة المركبات  السائقين / المستودعات / المخازن المرتبطة بالإسطول ؟", yes, cancel, function (result) {
                                if (result) {
                                    showLoader();
                                    axios({
                                        method: 'post',
                                        url: hostName + '/Fleets/UnlinkWithWaslAll/' + Id,
                                        headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                                    })
                                        .then(function (response) {
                                            if (!response.data.isSuccess) {
                                                SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                                            } else {
                                                SwalAlertOk("success", "تم إلغاء الربط بنجاح", "", function (result) {
                                                    setTimeout(function () {
                                                        window.location.reload();
                                                    }, 500);
                                                });
                                            }
                                            hideLoader();
                                        }).catch(function (error) {
                                            if (error.response.data.errorList) {
                                                var errorsHtml = "";
                                                $.each(error.response.data.errorList, function (index, error) {
                                                    errorsHtml += error;
                                                    errorsHtml += "<hr class='my-1'>";
                                                });
                                                SwalAlert("error", "فشل فك الربط", errorsHtml);
                                            } else {
                                                SwalAlert("error", "حدث خطأ في النظام", "");
                                            }
                                        });
                                }
                            });
                        }
                        else {
                            SwalAlert("error", "", error.response.data.errorList[0]);
                        }
                    }
                    else {
                        SwalAlert("error", "حدث خطأ في النظام", "");
                    }
                    hideLoader();
                });
        }
    });
}

function ConfirmUpdateContactInfo() {
    Save();
}

function SelectFleetType(selectedType) {
    if (selectedType == "company") {
        $(".companyRow").css("display", "");
        $(".individualRow").css("display", "none");
        if (!$(".vertical-wizard .company").hasClass("active")) {
            $(".vertical-wizard .company").addClass("active");
        }
        $(".vertical-wizard .individual").removeClass("active");
        $("#fleetType").val("company");
    } else {
        $(".individualRow").css("display", "");
        $(".companyRow").css("display", "none");
        if (!$(".vertical-wizard .individual").hasClass("active")) {
            $(".vertical-wizard .individual").addClass("active");
        }
        $(".vertical-wizard .company").removeClass("active");
        $("#fleetType").val("individual");
    }
}