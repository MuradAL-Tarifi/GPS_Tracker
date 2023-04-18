var PageSize = 100;
var PageNumber = 1;
var SearchString;
var IsActive;
var selectedUserId;

$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {

    InitFootable();

    const urlParams = new URLSearchParams(window.location.search);

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


    $('#searchInput').keyup(function (e) {
        if (e.keyCode == 13)
            Search();
    });

    $('#searchbtn').click(function () {
        Search();
    });

    $('#EnablePrivilegesbtn').click(function () {
        EnablePrivileges();
    });
});

function InitFootable() {
    FooTable.init("#tbUsers");
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
    PageNumber = 1;
    IsActive = $("#StatusSelect").val();
    GetData();
}

function GetData() {
    $.ajax({
        url: hostName + "/Users",
        data: {isActive: IsActive, search: SearchString, show: PageSize, page: PageNumber },
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

function ConfirmDelete(Id, pageNumber) {
    SwalConfirm("warning", text, "", yes, cancel, function (result) {
        if (result) {
            showLoader();
            var returnURL = hostName + '/Users?search=' + SearchString + '&show=' + PageSize + '&page=' + pageNumber;
            $("#returnURL").val(returnURL);
            $("#ItemId").val(Id);
            $("#DeleteForm").submit();
        }
    });
}

function refreshColors() {
    var rows = $("[id^=row_]");
    $.each(rows, function (index, row) {
        var id = row.id.split('_')[1];
        var exp = $("#exp_" + id).val();
        var dtId = $("#dtId_" + id).val();

        var expLabel = $("#expLabel_" + id);
        var accountLabel = $("#accountLabel_" + id);

        $(row).addClass(ExpRowBgClass(exp));
        expLabel.addClass(ExpBgClass(exp));
        accountLabel.addClass(DeviceTypeBgClass(parseInt(dtId)));
    });
}

function createUser(pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Users?search=' + SearchString + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Users/Create?returnURL=' + returnURL;
}

function editUser(id, pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Users?search=' + SearchString + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Users/Edit/' + id + '?returnURL=' + returnURL;
}

function viewUser(id, pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Users?search=' + SearchString + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Users/Details/' + id + '?returnURL=' + returnURL;
}

function manageUserPrivileges(id, pageNumber) {
    var returnURL = encodeURIComponent(hostName + '/Users?search=' + SearchString + '&show=' + PageSize + '&page=' + pageNumber);
    window.location.href = hostName + '/Users/ManagePrivileges/' + id + '?returnURL=' + returnURL;
}

function Login(username, password) {
    var loginUrl = hostName.replace("GPSTracker", "GPSTrackerAgent") + '/Account/SignInUser?Username=' + username + '&Password=' + password;
    const link = document.createElement('a');
    link.href = loginUrl;
    link.target = "_blank";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    //console.log(loginUrl);
}
