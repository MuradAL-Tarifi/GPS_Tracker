var PageSize = 100;
var PageNumber = 1;
var SearchString;
var BrandId;
var SensorStatus;

$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    $("#BrandsSelect").val("0");
    InitFootable();

    const urlParams = new URLSearchParams(window.location.search);
    BrandId = urlParams.get('brandId');
    if (!BrandId) {
        BrandId = '';
    }
    SensorStatus = urlParams.get('sensorStatus');
    if (!SensorStatus) {
        SensorStatus = '';
    }else {
        $("#SensorStatusSelect").val(SensorStatus);
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

    //$("#BrandsSelect").on("change", function () {
    //    // get sensors
    //    var brandId = $(this).val();
    //    if (brandId > 0) {
    //        BrandId = brandId;
    //    }
    //});

    $('#searchInput').keyup(function (e) {
        if (e.keyCode == 13)
            Search();
    });

    $('#searchbtn').click(function () {
        Search();
    });
    $('#exportbtn').click(function () {
        ExportToExcel();
    });
});

function InitFootable() {
    FooTable.init("#tbSensors");
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
    BrandId = $("#BrandsSelect").val();
    SensorStatus = $("#SensorStatusSelect").val();
    GetData();
}
function ExportToExcel() {
    var text = $('#searchInput').val().trim();
    SearchString = text;
    BrandId = $("#BrandsSelect").val();
    SensorStatus = $("#SensorStatusSelect").val();
    ExportToExcelData();
}
function GetData() {
    $.ajax({
        url: hostName + '/Reports/Sensor',
        data: { SensorStatus: SensorStatus, BrandId: BrandId, search: SearchString, show: PageSize, page: PageNumber },
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#PagedDataDiv').html(result);
            InitFootable();
        }
    });
}

function ExportToExcelData() {

axios({
    method: 'get',
    url: hostName + '/Reports/Sensor/ExportSensorsData',
    responseType: 'blob', // important
    data: {
        SensorStatus: SensorStatus, BrandId: BrandId, search: SearchString 
    }
}).then(function (response) {
    debugger;
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', 'WorkingAndNotWorkingSensorReport' + '.xlsx');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
})
    .catch(function (error) {
        if (error.response.status != 404) {
            alertify.notify(ErrorMessage, 'error').dismissOthers();
        }
    });
}
function PagedListSuccess() {
    InitFootable();
}


