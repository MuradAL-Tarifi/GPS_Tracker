var PageSize = 100;
var SearchString;
var Type;
var FromDate;
var ToDate;

$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    FlatPickerDate();

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
    FooTable.init("#tbOperations");
}


function ChangeSize(size) {
    PageSize = size;
    GetData();
}

function Search() {
    var text = $('#searchInput').val().trim();
    SearchString = text;
    PageSize = 100;
    Type = $("#EventTypesSelect").val();
    FromDate = $("#FromDateText").val();
    ToDate = $("#ToDateText").val();
    GetData();
}

function GetData() {
    $.ajax({
        url: hostName + '/Operations',
        data: { type: Type, fromDate: FromDate, toDate: ToDate, search: SearchString, show: PageSize },
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

function showContent(type, content) {
    $('#ContentDiv').html(type + "<br>" + JSON.stringify(content, null, 4));
    $('#ContentModal').modal();

}