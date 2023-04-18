var PageSize = 100;
var PageNumber = 1;
var SearchString;
var AgentId;
var FleetId;
var waslLinkStatus;
var isActive;

$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    InitFootable();

    const urlParams = new URLSearchParams(window.location.search);
    AgentId = urlParams.get('agentId');
    if (!AgentId) {
        AgentId = '';
    }

    FleetId = urlParams.get('fleetId');
    if (!FleetId) {
        FleetId = '';
    }

    SearchString = urlParams.get('search');
    if (!SearchString) {
        SearchString = '';
    }
    $('#searchInput').val(SearchString);

    PageSize = urlParams.get('show');
    if (!PageSize) {
        PageSize = 100;
    }
    PageNumber = urlParams.get('page');
    if (!PageNumber) {
        PageNumber = 1;
    }

    waslLinkStatus = urlParams.get('waslLinkStatus');
    if (!waslLinkStatus) {
        waslLinkStatus = '';
    } else {
        $("#LinkStatusSelect").val(waslLinkStatus);
    }

    isActive = urlParams.get('isActive');
    if (!isActive) {
        isActive = '';
    } else {
        $("#StatusSelect").val(isActive);
    }

    $("#AgentsSelect").on("change", function () {
        var agentId = $(this).val();
        if (agentId > 0) {
            AgentId = agentId;
            // Get Fleets
            $.get(hostName + '/Warehouse/GetFleets', { AgentId: agentId }).done(function (data) {
                var items = JSON.parse(data);
                var s = '';
                if (items.length > 0) {
                    s += '<option value="0">' + all + '</option>';
                }
                $.each(items, function (index, val) {
                    s += '<option value="' + val.Value + '">' + val.Text + '</option>';
                });
                $("#FleetsSelect").html(s);
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
        var fleetId = $(this).val();
        if (fleetId > 0) {
            FleetId = fleetId
        }
    });

    $('#searchInput').keyup(function (e) {
        if (e.keyCode == 13)
            Search();
    });

    $('#searchbtn').click(function () {
        Search();
    });
});

function InitFootable() {
    FooTable.init("#tbWarehouses");
}

function ConfirmDelete(Id, pageNumber) {
    SwalConfirm("warning", text, "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            var returnURL = hostName + '/Warehouse?agentId=' + AgentId + '&fleetId=' + FleetId + '&search=' + SearchString + '&waslLinkStatus=' + waslLinkStatus + '&isActive=' + isActive + '&show=' + PageSize + '&page=' + pageNumber;
            $("#returnURL").val(returnURL);
            $("#ItemId").val(Id);
            $("#DeleteForm").submit();
        }
    });
}

function ConfirmLinkWithWasl(Id, pageNumber) {
    PageNumber = pageNumber;
    SwalConfirm("warning", "هل انت متأكد ؟", "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            axios({
                method: 'post',
                url: hostName +'/Warehouse/LinkWithWasl/' + Id,
                headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
            })
                .then(function (response) {
                    if (!response.data.isSuccess) {
                        SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                    } else {
                        SwalAlertOk("success", "تم الربط بنجاح", "", function (result) {
                            setTimeout(function () {
                                GetData();
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

function ConfirmUnlinkWithWasl(Id, pageNumber) {
    PageNumber = pageNumber;
    SwalConfirm("warning", "هل انت متأكد ؟", "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            axios({
                method: 'post',
                url: hostName + '/Warehouse/UnlinkWithWasl/' + Id,
                headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
            })
                .then(function (response) {
                    if (!response.data.isSuccess) {
                        SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                    } else {
                        SwalAlertOk("success", "تم إلغاء الربط بنجاح", "", function (result) {
                            setTimeout(function () {
                                GetData();
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

function ChangeSize(size) {
    PageSize = size;
    GetData();
}

function Search() {
    var text = $('#searchInput').val().trim();
    SearchString = text;
    PageSize = 100;
    PageNumber = 1;
    AgentId = $("#AgentsSelect").val();
    FleetId = $("#FleetsSelect").val();
    waslLinkStatus = $("#LinkStatusSelect").val();
    isActive = $("#StatusSelect").val();
    GetData();
}

function GetData() {
    $.ajax({
        url: hostName + '/Warehouse',
        data: { agentId: AgentId, fleetId: FleetId, search: SearchString, waslLinkStatus: waslLinkStatus, isActive: isActive, show: PageSize, page: PageNumber },
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#PagedDataDiv').html(result);
            InitFootable();
        }
    });
}

function PagedListSuccess() {
    InitFootable();
}

function createWarehouse(pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Warehouse?agentId=' + AgentId + '&fleetId=' + FleetId + '&search=' + SearchString + '&waslLinkStatus=' + waslLinkStatus + '&isActive=' + isActive + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Warehouse/create?agentId=' + AgentId + '&fleetId=' + FleetId + '&returnURL=' + returnURL;
}

function editWarehouse(id, pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Warehouse?agentId=' + AgentId + '&fleetId=' + FleetId + '&search=' + SearchString + '&waslLinkStatus=' + waslLinkStatus + '&isActive=' + isActive + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Warehouse/create/' + id + '?returnURL=' + returnURL;
}

function viewWarehouse(id, pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Warehouse?agentId=' + AgentId + '&fleetId=' + FleetId + '&search=' + SearchString + '&waslLinkStatus=' + waslLinkStatus + '&isActive=' + isActive + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Warehouse/details/' + id + '?returnURL=' + returnURL;
}