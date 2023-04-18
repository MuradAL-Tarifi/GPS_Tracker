

var app = new Vue({
    el: '#app',
    data: {
        searchFilter: {
            pageSize: 100,
            searchString: '',
            reportTypeId: 0
        },
        reportTypes: []
    },
    created: function () {
        alertify.set('notifier', 'position', 'bottom-center');
        var self = this;

        window.ChangeSize = function (size) {
            self.ChangeSize(size);
        }

        window.PagedListSuccess = function () {
            self.PagedListSuccess();
        }

        window.ConfirmDelete = function (id) {
            self.ConfirmDelete(id);
        }

        window.ConfirmActivate = function (id) {
            self.confirmActivate(id);
        }

        window.ConfirmDeActivate = function (id) {
            self.confirmDeActivate(id);
        }
    },
    mounted: function () {
        var self = this;
        setTimeout(function () { self.initialize(); }, 500);
    },
    methods: {
        initialize: function () {
            var self = this;
            axios.get(hostName + '/api/ReportTypeLookup')
                .then(function (response) {
                    self.reportTypes = response.data;
                })
                .catch(function (error) {
                    alertify.notify(ErrorMessage, 'error').dismissOthers();
                });

            self.InitFootable();
        },
        InitFootable: function () {
            FooTable.init("#tbReports");
        },
        ChangeSize: function (size) {
            this.pageSize = size;
            this.GetData();
        },
        Search: function (size) {
            this.pageSize = 100;
            this.GetData();
        },
        GetData: function () {
            var self = this;
            $.ajax({
                url: hostName + "/Scheduling",
                data: {
                    reportTypeId: self.searchFilter.reportTypeId,
                    search: self.searchFilter.searchString,
                    show: self.searchFilter.pageSize
                },
                type: 'GET',
                cache: false,
                success: function (result) {
                    $('#PagedDataDiv').html(result);
                    self.InitFootable();
                }
            });
        },
        PagedListSuccess: function () {
            this.InitFootable();
        },
        ConfirmDelete: function (Id) {
            SwalConfirm("warning", text, "", yes, cancel, function (result) {
                if (result) {
                    $("#ItemId").val(Id);
                    $("#DeleteForm").submit();
                }
            });
        },
        confirmActivate: function (Id) {
            SwalConfirm("warning", "هل انت متأكد ؟", "", yes, cancel, function (result) {
                if (result) {
                    axios({
                        method: 'post',
                        url: hostName + '/Reports/Scheduling/Activate/' + Id,
                        headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                    })
                        .then(function (response) {
                            if (!response.data.isSuccess) {
                                SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                            } else {
                                SwalAlertOk("success", "تم التفعيل بنجاح", "", null);
                                setTimeout(function () {
                                    window.location.reload();
                                }, 500);
                            }
                        })
                        .catch(function (error) {
                            if (error.response.data.errorList) {
                                SwalAlert("error", "", error.response.data.errorList[0]);
                            }
                            else {
                                SwalAlert("error", "حدث خطأ في النظام", "");
                            }
                        });
                }
            });
        },
        confirmDeActivate: function (Id) {
            SwalConfirm("warning", "هل انت متأكد ؟", "", yes, cancel, function (result) {
                if (result) {
                    axios({
                        method: 'post',
                        url: hostName + '/Reports/Scheduling/DeActivate/' + Id,
                        headers: { 'Content-Type': 'multipart/form-data', 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                    })
                        .then(function (response) {
                            if (!response.data.isSuccess) {
                                SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                            } else {
                                SwalAlertOk("success", "تم التعطيل بنجاح", "", null);
                                setTimeout(function () {
                                    window.location.reload();
                                }, 500);
                            }
                        })
                        .catch(function (error) {
                            if (error.response.data.errorList) {
                                SwalAlert("error", "", error.response.data.errorList[0]);
                            }
                            else {
                                SwalAlert("error", "حدث خطأ في النظام", "");
                            }
                        });
                }
            });
        }
    }
});