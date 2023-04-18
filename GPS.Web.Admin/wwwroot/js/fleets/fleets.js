var PageSize = 100;
var SearchString;
var waslLinkStatus;


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

    const urlParams = new URLSearchParams(window.location.search);
    waslLinkStatus = urlParams.get('waslLinkStatus');
    if (!waslLinkStatus) {
        waslLinkStatus = '';
    } else {
        $("#LinkStatusSelect").val(waslLinkStatus);
    }

});

function InitFootable() {
    FooTable.init("#tbFleets");
}

function ChangeSize(size) {
    PageSize = size;
    GetData();
}

function Search() {
    var text = $('#searchInput').val().trim();
    SearchString = text;
    PageSize = 100;
    waslLinkStatus = $("#LinkStatusSelect").val();
    GetData();
}

function GetData() {
    $.ajax({
        url: hostName +'/Companies/Index',
        data: { search: SearchString, waslLinkStatus: waslLinkStatus, show: PageSize },
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
            $("#ItemId").val(Id);
            $("#DeleteForm").submit();
        }
    });
}