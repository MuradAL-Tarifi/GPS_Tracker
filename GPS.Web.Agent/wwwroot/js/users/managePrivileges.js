
$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {

    $('#savebtn').click(function () {
        var selectedUserId = $("#userId").val();
        var returnUrl = $("input[name='returnURL']").val();
        if (selectedUserId) {
            var Privileges = [];
            var checkboxes = $("[id^=privilegeType_]");
            $.each(checkboxes, function (index, checkbox) {
                var PrivilegeTypeId = checkbox.id.split('_')[1];
                var IsActive = $(checkbox).is(":checked");
                var privilege = { PrivilegeTypeId: PrivilegeTypeId, IsActive: IsActive };
                Privileges.push(privilege);
            });

            Swal.fire({
                title: confirmSavePrivileges,
                type: "question",
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#d33',
                confirmButtonText: yes,
                cancelButtonText: cancel,
                animation: false,
                showLoaderOnConfirm: true,
                preConfirm: () => {
                    return updateUserPrivileges(selectedUserId, Privileges).then(response => {
                        if (!response) {
                            SwalAlert('error', updatePrivilegesFailed, '');
                        }
                        return response;
                    });
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (result.value) {
                    SwalFlipAlertOk('success', updatePrivilegesSuccess, '', function () {
                        window.location.href = returnUrl;
                    });
                }
            });
        }
    });
});



async function updateUserPrivileges(selectedUserId, Privileges) {
    var User = { Id: selectedUserId};
    var data = { User: User, Privileges: Privileges };
    const result = await $.ajax({
        type: "POST",
        url: hostName + "/Users/UpdateUserPrivileges",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });

    return result;
}

function checkAll(checked) {
    if (checked) {
        $("[id^=privilegeType_]").prop('checked', true);
    }
    else {
        $("[id^=privilegeType_]").prop('checked', false);
    }
}