$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    $("#AgentsSelect").on("change", function () {
        // get groups
        var agentId = $(this).val();
        if (agentId > 0) {
            $.get(hostName +'/Groups/GetFleets', { AgentId: agentId }).done(function (data) {
                var items = JSON.parse(data);
                var s = '';
                if (items.length > 0) {
                    s += '<option value="0">' + selectFleet + '</option>';
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
});



function ConfirmDelete() {
    SwalConfirm("warning", text, "", yes, cancel, function (result) {
        if (result) {
            $("#DeleteForm").submit();
        }
    });
}