var PageSize = 100;
var SearchString;

$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    InitFootable();

    $('#searchInput').keyup(function (e) {
        if (e.keyCode == 13)
            Search();
    });

    $('#searchbtn').click(function () {
        Search();
    });
});

function InitFootable() {
    FooTable.init("#tbGateways");
    refreshColors();
}

function ChangeSize(size) {
    PageSize = size;
    GetData();
}

function Search() {
    var text = $('#searchInput').val().trim();
    SearchString = text;
    PageSize = 100;
    GetData();
}

function GetData() {
    $.ajax({
        url: hostName +'/Gateway',
        data: { search: SearchString, show: PageSize },
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

function ConfirmDelete(Id) {
    SwalConfirm("warning", text, "", yes, cancel, function (result) {
        if (result) {
            $.ajax({
                url: hostName + '/Gateway/IsGatewayLinkedToInventory',
                data: { GatewayId: Id },
                type: 'GET',
                cache: false,
                success: function (isLinkedToInventory) {
                    if (!isLinkedToInventory) {
                        $("#ItemId").val(Id);
                        $("#DeleteForm").submit();
                    } else {
                        SwalAlert("error", "", gatewayLinkedWithInventory);
                    }
                   
                }
            });
               
        }
    });
}


function refreshColors() {
    var rows = $("[id^=row_]");
    $.each(rows, function (index, row) {
        var id = row.id.split('_')[1];
        var exp = $("#exp_" + id).val();

        var expLabel = $("#expLabel_" + id);

        expLabel.addClass(ExpBgClass(exp));
    });
}
