$(document).ajaxStart(function () { Pace.restart(); });
Vue.use(window.vuelidate.default);
// register globally
Vue.component('v-select', VueSelect.VueSelect)



var app = new Vue({
    el: "#app",
    data: {
        formObj: {
            id: 0,
            title: '',
            alertTypeLookupId: 0,
            minValueHumidity: null,
            maxValueHumidity: null,
            minValueTemperature: null,
            maxValueTemperature: null,
            interval: null,
            toEmails: '',
            optionWarehouses: [],
            optionInventories: [],
            optionUsers: [],
            selectedWarehouse: null,
            selectedInventories: [],
            selectedUsers: [],
            isActive: true,
            warehouseId: null
        },
        savePending: false,
        isEditing: false,
        isOptionWarehousesMultiple: false,
        isOptionInventoriesMultiple: true,
        headerAddCustomAlert: addCustomAlert,
        headerEditCustomAlert: editCustomAlert,
        regEmail: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+([a-zA-Z]{2,24})*$/,
        minInterval: MinInterval,
        searchFilter: {
            warehouseId: 0,
            selectedInventory: null,
            isActive: -1,
            inventories: [],
            searchString: '',
            pageNumber: null,
            pageSize: 100,
        },
        enableAlertByUsers:false
    },
    methods: {
        getSelectInventories: function () {
            var self = this;
            self.searchFilter.inventories = [];
            self.getInventories().then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.searchFilter.inventories.push({ text: response.data[i].text, value: response.data[i].value });
                }

                self.searchFilter.inventories.map(function (x) {
                    return x.inventorySelectLabel = x.text;
                });
            })
            .catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(errorMessage, 'error').dismissOthers();
            });
        },
        getFormSelectInventories: function (warehouseId = 0) {
            var self = this;
            self.formObj.optionInventories = [];
            self.formObj.selectedInventories = [];
            self.formObj.warehouseId = warehouseId;
            self.getInventories(warehouseId).then(function (response) {
                if (response.data.length > 0) {
                    self.formObj.optionInventories.push({ text: all, value: 0 });
                }
                for (var i = 0; i < response.data.length; i++) {
                    self.formObj.optionInventories.push({ text: response.data[i].text, value: response.data[i].value });
                }
                self.formObj.optionInventories.map(function (x) {
                    return x.inventorySelectLabel = x.text;
                });

            }).catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(errorMessage, 'error').dismissOthers();
            });
        },
        getFormSelectUsers: function () {
            var self = this;
            self.formObj.optionUsers = [];
            self.formObj.selectedUsers = [];

           axios.get(hostName + '/api/Users').then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.formObj.optionUsers.push({ text: response.data[i].text, value: response.data[i].value });
                }
                self.formObj.optionUsers.map(function (x) {
                    return x.userSelectLabel = x.text;
                });
            }).catch(function (error) {
                alertify.notify(errorMessage, 'error').dismissOthers();
            }).finally(() => {
                
            });
        },
        getFormSelectWarehouses: function () {
            var self = this;
            self.getWarehoues().then(function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    self.formObj.optionWarehouses.push({ text: response.data[i].text, value: response.data[i].value });
                }
                self.formObj.optionWarehouses.map(function (x) {
                    return x.warehouseSelectLabel = x.text;
                });

            }).catch(function (error) {
                //console.log("error =>" + error);
                alertify.notify(errorMessage, 'error').dismissOthers();
            });
        },
        alertByUsers: function () {
            var self = this;
            if (self.enableAlertByUsers) {
                self.getFormSelectUsers();
            }
        },
        getInventories: async function (warehouseId = 0) {
            return await axios.get(hostName + '/api/Inventories?warehouseId=' + (warehouseId > 0 ? warehouseId : self.warehouseId));
        },
        getWarehoues: async function () {
            return await axios.get(hostName + '/api/Warehouses');
        },
        onSave: function () {
            var self = this;
            self.$v.$touch();

            if (!self.$v.$invalid) {
                self.$v.$reset();
                self.savePending = true;
                var inventories = [];
                var users = [];
                for (var i = 0; i < self.formObj.selectedInventories.length; i++) {
                    inventories.push({ id: parseInt(self.formObj.selectedInventories[i].value) });
                }

                for (var i = 0; i < self.formObj.selectedUsers.length; i++) {
                    users.push(self.formObj.selectedUsers[i].value);
                }

               // Object.assign(self.formObj, { inventories: inventories });
               // Object.assign(self.formObj, { userIds: users.toString() });
                //1 HumidityOutOfRange
                if (self.formObj.alertTypeLookupId == 1) {
                    self.formObj.minValueTemperature = null;
                    self.formObj.maxValueTemperature = null;
                }
                //1 TemperatureOutOfRang
                if (self.formObj.alertTypeLookupId == 2) {
                    self.formObj.minValueHumidity = null;
                    self.formObj.maxValueHumidity = null;
                }
                var data =
                {
                    id: self.formObj.id == null ? 0 : self.formObj.id,
                    title: self.formObj.title,
                    isActive: self.formObj.isActive,
                    minValueHumidity: self.formObj.minValueHumidity,
                    maxValueHumidity: self.formObj.maxValueHumidity,
                    minValueTemperature: self.formObj.minValueTemperature,
                    maxValueTemperature: self.formObj.maxValueTemperature,
                    inventories: inventories,
                    alertTypeLookupId: self.formObj.alertTypeLookupId,
                    interval: self.formObj.interval,
                    toEmails: !self.enableAlertByUsers ? self.formObj.toEmails : null,
                    userIds: self.enableAlertByUsers ? users.toString() : null,
                    warehouseId: self.formObj.warehouseId
                };
                //console.log(data);
                axios.post(hostName + "/CustomAlert/Save", data)
                    .then(function (response) {
                        if (response.data.isSuccess) {
                            SwalAlert("success", "", saveSuccessMSG);
                            setTimeout(function () {
                                //window.location.reload();
                                $('#AddUpdateCustomAlertModal .close').click();
                                self.search();
                            }, 200);
                        }
                    })
                    .catch(function (error) {
                        if (error.response.data.errorList) {
                            SwalAlert("error", "", error.response.data.errorList[0]);
                        }
                        else {
                            SwalAlert("error", "", errorMessage);
                        }
                    }).finally(function () {
                        self.savePending = false;
                    });
            }
        },
        subtractOrAdd: function (objName, action) {
            var self = this;
            // Humidity
            if (objName == "minValueHumidity" && action == "subtract") {
                self.formObj.minValueHumidity = self.formObj.minValueHumidity == null ? 0 : self.formObj.minValueHumidity;
                if (self.formObj.minValueHumidity > 0)
                    self.formObj.minValueHumidity -= 1;
            }

            if (objName == "minValueHumidity" && action == "add") {
                self.formObj.minValueHumidity = self.formObj.minValueHumidity == null ? 0 : self.formObj.minValueHumidity;
                self.formObj.minValueHumidity += 1;
            }

            if (objName == "maxValueHumidity" && action == "subtract") {
                self.formObj.maxValueHumidity = self.formObj.maxValueHumidity == null ? 0 : self.formObj.maxValueHumidity;
                if (self.formObj.maxValueHumidity > 0)
                    self.formObj.maxValueHumidity -= 1;
            }

            if (objName == "maxValueHumidity" && action == "add") {
                self.formObj.maxValueHumidity = self.formObj.maxValueHumidity == null ? 0 : self.formObj.maxValueHumidity;
                self.formObj.maxValueHumidity += 1;
            }


            // Temperature
            if (objName == "minValueTemperature" && action == "subtract") {
                self.formObj.minValueTemperature = self.formObj.minValueTemperature == null ? 0 : self.formObj.minValueTemperature;
                if (self.formObj.minValueTemperature > 0)
                    self.formObj.minValueTemperature -= 1;
            }

            if (objName == "minValueTemperature" && action == "add") {
                self.formObj.minValueTemperature = self.formObj.minValueTemperature == null ? 0 : self.formObj.minValueTemperature;
                self.formObj.minValueTemperature += 1;
            }

            if (objName == "maxValueTemperature" && action == "subtract") {
                self.formObj.maxValueTemperature = self.formObj.maxValueTemperature == null ? 0 : self.formObj.maxValueTemperature;
                if (self.formObj.maxValueTemperature > 0)
                    self.formObj.maxValueTemperature -= 1;
            }

            if (objName == "maxValueTemperature" && action == "add") {
                self.formObj.maxValueTemperature = self.formObj.maxValueTemperature == null ? 0 : self.formObj.maxValueTemperature;
                self.formObj.maxValueTemperature += 1;
            }

        },
        onSelectFormWarehouse: function (obj) {
            var self = this;
            var selectedWarehouseId = obj != null ? obj.value : 0;
            self.getFormSelectInventories(selectedWarehouseId);
        },
        initFootable: function () {
            FooTable.init("#tbCustomAlerts");
        },
        searchKeyUp: function (e) {
          var self = this;
            this.$nextTick(() => {
            if (e.keyCode == 13) {
                  self.search();
                 }
            });
        },
        search: function () {
            //console.log(this.searchFilter);
            var self = this;
            $.ajax({
                url: hostName + "/CustomAlert",
                data: {
                    warehouseId: self.searchFilter.warehouseId,
                    inventoryId: self.searchFilter.selectedInventory == null ? null : self.searchFilter.selectedInventory.value,
                    isActive: self.searchFilter.isActive,
                    search: self.searchFilter.searchString,
                    show: self.searchFilter.pageSize,
                    page: self.searchFilter.pageNumber
                },
                type: 'GET',
                cache: false,
                success: function (result) {
                    $('#PagedDataDiv').html(result);
                    self.initFootable();
                }
            });
        },
        onInventorySelect: function (obj) {
            if (obj.length > 1) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].value == 0) {
                        this.formObj.selectedInventories = this.formObj.selectedInventories.filter(x => {
                            return (x.value == obj[i].value);
                        });
                    }
                }
            }
        }
    },
    validations: {
        formObj: {
            title: {
                required: validators.required,
                maxLength: validators.maxLength(100)
            },
            alertTypeLookupId: {
                validSelectedAlertType(value) {
                    if (value > 0) {
                        return true;
                    }
                    return false;
                }
            },
            minValueHumidity: {
                required: validators.requiredIf(function (x) { return (this.formObj.alertTypeLookupId == '1' || this.formObj.alertTypeLookupId == '3') && !this.formObj.minValueHumidity }),
            },
            maxValueHumidity: {
                required: validators.requiredIf(function (x) { return (this.formObj.alertTypeLookupId == '1' || this.formObj.alertTypeLookupId == '3') && !this.formObj.maxValueHumidity }),
            },
            minValueTemperature: {
                required: validators.requiredIf(function (x) { return (this.formObj.alertTypeLookupId == '2' || this.formObj.alertTypeLookupId == '3') && !this.formObj.minValueTemperature }),
            },
            maxValueTemperature: {
                required: validators.requiredIf(function (x) { return (this.formObj.alertTypeLookupId == '2' || this.formObj.alertTypeLookupId == '3') && !this.formObj.maxValueTemperature }),
            },
            interval: {
                required: validators.required,
                minInterval(interval) {
                    var self = this;
                    if (!interval) {
                        return true;
                    }
                    return interval >= self.minInterval;
                }
            },
            toEmails: {
                required: validators.requiredIf(function (x) { return !this.enableAlertByUsers }),
                maxLength: validators.maxLength(500),
                validEmails(emailsString) {
                    var self = this;
                    if (!emailsString) {
                        return true;
                    }
                    var emails = emailsString.split(",");
                    for (i = 0; i < emails.length; i++) {
                        if (!self.regEmail.test(emails[i])) {
                            return false;
                        }
                    }
                    return true;
                }
            },
            selectedWarehouse: {
                required: validators.required,
            },
            selectedInventories: {
                required: validators.required,
            },
            selectedUsers: {
                required: validators.requiredIf(function (x) { return this.enableAlertByUsers })
            }
        },
    },
    created: function () {
            var self = this;

            window.ChangeSize = function (size) {
                self.searchFilter.pageSize = size;
                self.search();
            }
            window.PagedListSuccess = function () {
                self.initFootable();
            }
            window.onEdit = async function  (id, pageNumber) {
                if (id > 0) {
                    self.getFormSelectUsers();
                    axios.get(hostName + '/CustomAlert/GetCustomAlertById?customAlertId=' + id)
                        .then(function (response) {
                            if (response.data.isSuccess) {
                                //console.log(response.data.data);
                                self.pageNumber = pageNumber;
                                self.isEditing = true;

                                self.formObj.optionWarehouses = [];
                                self.formObj.selectedWarehouse = null;
                                self.formObj.optionInventories = [];
                                self.formObj.selectedInventories = [];

                                if (response.data.data.userIds != null) {
                                    var users = [];
                                    users = response.data.data.userIds.split(",");
                                    self.enableAlertByUsers = true;
                                    
                                    //console.log(self.formObj.optionUsers);
                                    self.formObj.selectedUsers = self.formObj.optionUsers.filter(item => users.includes(item.value));
                                }
                                self.getFormSelectWarehouses();
                                self.getFormSelectInventories(response.data.data.inventories[0].warehouseId);

                                response.data.data.inventories.forEach((inventory) => {
                                    self.formObj.selectedInventories.push({ text: inventory.name, value: inventory.id.toString(), inventorySelectLabel: inventory.name });
                                });
                                self.formObj.selectedWarehouse = {
                                    text: response.data.data.inventories[0].warehouse.name,
                                    value: response.data.data.inventories[0].warehouse.id,
                                    warehouseSelectLabel: response.data.data.inventories[0].warehouse.name
                                };
                                self.formObj.id = response.data.data.id;
                                self.formObj.title = response.data.data.title;
                                self.formObj.alertTypeLookupId = response.data.data.alertTypeLookupId;
                                self.formObj.minValueHumidity = response.data.data.minValueHumidity;
                                self.formObj.maxValueHumidity = response.data.data.maxValueHumidity;
                                self.formObj.minValueTemperature = response.data.data.minValueTemperature;
                                self.formObj.maxValueTemperature = response.data.data.maxValueTemperature;
                                self.formObj.interval = response.data.data.interval;
                                self.formObj.toEmails = response.data.data.toEmails;
                                self.formObj.isActive = response.data.data.isActive;
                                //console.log(self.formObj);
                                $("#AddUpdateCustomAlertModal").modal();

                            }
                        })
                        .catch(function (error) {
                            alertify.notify(errorMessage, 'error').dismissOthers();
                        });

                }
            }
            window.onAdd = function (pageNumber) {
                self.pageNumber = pageNumber;
                self.isEditing = false;
                self.formObj.id = null;
                self.formObj.title = null;
                self.formObj.alertTypeLookupId = 0;
                self.formObj.optionWarehouses = [];
                self.formObj.selectedWarehouse = null;
                self.formObj.optionInventories = [];
                self.formObj.selectedInventories = [];
                self.formObj.selectedUsers = [];
                self.formObj.minValueHumidity = null;
                self.formObj.maxValueHumidity = null;
                self.formObj.minValueTemperature = null;
                self.formObj.maxValueTemperature = null;
                self.formObj.interval = null;
                self.formObj.toEmails = null;
                self.getFormSelectWarehouses();
                $("#AddUpdateCustomAlertModal").modal();

            }

        window.onDelete = function (id, pageNumber) {
            SwalConfirm("warning", confirmDelete, "", yes, cancel, function (result) {
                if (result) {
                    self.savePending = true;
                    self.pageNumber = pageNumber;
                    axios.post(hostName + "/CustomAlert/Delete/" + id)
                        .then(function (response) {
                            if (response.data.isSuccess) {
                                SwalAlert("success", "", deleteSuccessMSG);
                                self.search();
                            }
                        })
                        .catch(function (error) {
                            if (error.response.data.errorList) {
                                SwalAlert("error", "", error.response.data.errorList[0]);
                            }
                            else {
                                SwalAlert("error", "", errorMessage);
                            }
                        }).finally(function () {
                            self.savePending = false;
                        });
                }
            });

        }

    },
    mounted: function() {
        var self = this;
        self.getSelectInventories();
    }
});