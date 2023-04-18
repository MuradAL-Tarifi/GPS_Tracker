$(document).ready(function () {
    refreshColors();
    WarehousesInventoriesJsTree();
});



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

