var selectedSensorIds;


$(document).ready(function () {
    $('#EnableAlertsbtn').click(function () {
        EnableAlerts();
    });

    $("#CheckboxAll").on("change", function () {
        if ($(this).is(":checked")) {
            $("[id^=SeonsorCheckbox]").prop('checked', true);
        }
        else {
            $("[id^=SeonsorCheckbox]").prop('checked', false);
        }
    });
});


function EnableAlerts() {
    selectedSensorIds = [];
    var checkboxes = $("[id^=SeonsorCheckbox]");
    $.each(checkboxes, function (index, checkbox) {
        if ($(checkbox).is(":checked")) {
            var sensorId = checkbox.id.split('_')[1];
            selectedSensorIds.push(parseInt(sensorId));
        }
    });

    if (selectedSensorIds.length > 0) {
        $.get(hostName + "/Inventory/GetSensorAlerts", { sensorIdsString: JSON.stringify(selectedSensorIds) }).done(function (data) {
            var list = JSON.parse(data);
            var rows = '';
            $.each(list, function (index, sensorAlert) {
                var IsActive = sensorAlert.IsActive;
                var IsSMS = sensorAlert.IsSMS;
                var IsEmail = sensorAlert.IsEmail;
                var AlertName = isEnglish == 'True' ? sensorAlert.SensorAlertTypeLookup.NameEn : sensorAlert.SensorAlertTypeLookup.Name;
                var FromValue = sensorAlert.FromValue ?? "";
                var ToValue = sensorAlert.ToValue ?? "";

                rows += '<tr id="alertType_row_' + sensorAlert.SensorAlertTypeLookupId + '">';
                rows += ('<td><input id="alertIsActive_' + sensorAlert.SensorAlertTypeLookupId + '" type="checkbox" ' + (IsActive ? "checked" : "") + ' class="is-active alert-checkbox" data-size="normal" data-toggle="toggle"  data-onstyle="success" data-offstyle="danger" data-style="alert-check"/></td>');
                rows += ('<td>' + AlertName + '</td>');

                if (sensorAlert.SensorAlertTypeLookup.IsRange) {
                    rows += ('<td><input id="alertFromValue_' + sensorAlert.SensorAlertTypeLookupId + '" type="' + (sensorAlert.SensorAlertTypeLookup.DataType == "time" ? "time" : "number") + '" value="' + FromValue + '" class="from-value" /></td>');
                    rows += ('<td><input id="alertToValue_' + sensorAlert.SensorAlertTypeLookupId + '" type="' + (sensorAlert.SensorAlertTypeLookup.DataType == "time" ? "time" : "number") + '" value="' + ToValue + '" class="to-value" /></td>');
                }
                else {
                    rows += '<td></td><td></td>';
                }

                rows += ('<td><input id="alertIsSMS_' + sensorAlert.SensorAlertTypeLookupId + '" type="checkbox" ' + (IsSMS ? "checked" : "") + ' class="is-sms alert-checkbox" data-size="normal" data-toggle="toggle"  data-onstyle="success" data-offstyle="danger" data-style="alert-check"/></td>');
                rows += ('<td><input id="alertIsEmail_' + sensorAlert.SensorAlertTypeLookupId + '" type="checkbox" ' + (IsEmail ? "checked" : "") + ' class="is-email alert-checkbox" data-size="normal" data-toggle="toggle"  data-onstyle="success" data-offstyle="danger" data-style="alert-check"/></td>');


                rows += '</tr>';
            });

            $("#SensorAlertTypesTable tbody").html(rows);
        }).then(function () {
            $(".alert-checkbox").bootstrapToggle({
                on: '&#10004;',
                off: 'x',
                size: 'mini'
            });
            $('#SensorAlertTypesModal').modal();
        });
    }
}

$('#saveSensorAlerts').click(function () {
    if (selectedSensorIds.length > 0) {
        var sensorAlerts = [];
        var rows = $("[id^=alertType_row_]");
        $.each(rows, function (index, row) {
            var sensorAlertTypeLookupId = row.id.split('_')[2];
            var isActive = $(row).find(".is-active").is(":checked");
            var isSMS = $(row).find(".is-sms").is(":checked");
            var isEmail = $(row).find(".is-email").is(":checked");
            var fromValue = $(row).find(".from-value").val();
            var toValue = $(row).find(".to-value").val();

            var alert = {
                SensorAlertTypeLookupId: sensorAlertTypeLookupId,
                IsActive: isActive,
                IsSMS: isSMS,
                IsEmail: isEmail,
                FromValue: fromValue,
                ToValue: toValue,
            };

            sensorAlerts.push(alert);
        });

        Swal.fire({
            title: confirmSaveAlerts,
            type: "question",
            showCancelButton: true,
            confirmButtonColor: '#28a745',
            cancelButtonColor: '#d33',
            confirmButtonText: yes,
            cancelButtonText: cancel,
            animation: false,
            showLoaderOnConfirm: true,
            preConfirm: () => {
                return updateSensorAlerts(sensorAlerts).then(response => {
                    if (!response) {
                        SwalAlert('error', updateAlertsFailed, '');
                    }
                    return response;
                });
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            if (result.value) {
                SwalAlertOk('success', updateAlertsSuccess, '', function () {
                    $('#SensorAlertTypesModal .close').click();
                });
            }
        });
    }
});

async function updateSensorAlerts(sensorAlerts) {
    var data = { SensorIds: selectedSensorIds, SensorAlerts: sensorAlerts };
    const result = await $.ajax({
        type: "POST",
        url: hostName + "/Inventory/UpdateSensorAlerts",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });

    return result;
}