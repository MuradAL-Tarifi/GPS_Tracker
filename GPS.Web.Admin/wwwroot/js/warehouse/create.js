$(document).ajaxStart(function () { Pace.restart(); });
Vue.use(window.vuelidate.default);

$(document).ready(function () {
    FlatPickerDate();
});

Vue.component('v-select', VueSelect.VueSelect)

var app = new Vue({
    el: '#app',
    data: {
        hostName: hostName,
        dtpOptions: {
            format: 'YYYY-MM-DD',
            useCurrent: 'day',
            showClear: true,
            showClose: true,
            showTodayButton: true
        },
        isEdit: false,
        agentId: null,
        fleetId: '',
        agents: [],
        fleets: [],
        selectedFleet: null,
        savePending: false,
        warehouse: JSON.parse(JSON.stringify(warehouseModel)),
        gMap: null,
        locationMarker: null,
        drawingManager: null,
        drawing: false,
        landCoordinatesPolygon: null,
        returnURL: ReturnURL,
    },
    validations: {
        warehouse: {
            fleetId: { required: function (value) { return value > 0; } },
            name: { required: validators.required },
            //phone: { required: validators.required },
            //address: { required: validators.required },
            //city: { required: validators.required },
            //landAreaInSquareMeter: { required: function (value) { return value > 0; } },
            //latitude: { required: validators.required },
            //longitude: { required: validators.required },
            //licenseNumber: { required: validators.required },
            //licenseIssueDate: { required: validators.required },
            //licenseExpiryDate: { required: validators.required },
            //managerMobile: { required: validators.required },
            //email: { required: validators.required },
        },
    },
    created: function () {
        var self = this;
        self.GetAgents();
        if (self.warehouse.id > 0) {
            self.isEdit = true;
        }
        if (!self.isEdit) {
            self.warehouse.latitude = 24.725398;
            self.warehouse.longitude = 46.2619989;
        }
    },
    mounted: function () {
        var self = this;

        //axios.get(hostName + '/Warehouse/GetRegisterTypes')
        //    .then(function (response) {
        //        self.registerTypes = response.data;
        //    })
        //    .catch(function (error) { });

        setTimeout(function () { self.initMap(); }, 300);
    },
    methods: {
        initMap: function () {
            var self = this;

            var center = new google.maps.LatLng(24.712358, 46.667013);
            self.gMap = new google.maps.Map(document.getElementById('map'), {
                zoom: 18,
                center: center,
                gestureHandling: 'greedy',
                streetViewControl: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
            });

            if (self.isEdit) {
                var latLng = new google.maps.LatLng(self.warehouse.latitude, self.warehouse.longitude);
                self.setLocationMarker(latLng, true);
                self.setLandCoordinatesPolygon();
            }
        },
        GetAgents: function () {
            var self = this;
            axios.get(hostName + '/Warehouse/GetAgents/')
                .then(function (response) {
                    self.agents = response.data;
                    self.agentId = self.agents[0].Value;
                    self.GetFleets();
                   
                })
                .catch(function (error) { });
        },
        GetFleets: function () {
            var self = this;
            axios.get(hostName + '/Warehouse/GetFleets?AgentId=' + self.agentId)
                .then(function (response) {
                    self.fleets = response.data;
                    self.fleetId = self.warehouse.fleetId;
                })
                .catch(function (error) { });
        },
        onNext: function () {
            var self = this;
         
            self.$v.warehouse.$touch();
            if (!self.$v.warehouse.$invalid) {
                self.$v.$reset();

                var horizontalWizard = document.querySelector('.horizontal-wizard-example');
                var horizontalStepper = new Stepper(horizontalWizard, {
                    linear: true
                });

                horizontalStepper.next();
            }

        },
        onSaveWarehouse: function () {
            var self = this;

            self.$v.warehouse.$touch();
            if (!self.$v.warehouse.$invalid) {
                self.$v.$reset();

                SwalConfirm("question", confirmSave, "", yes, cancel, function (result) {
                    self.savePending = true;
                    axios({
                        method: 'post',
                        url: hostName + '/Warehouse/Create',
                        data: self.warehouse,
                        headers: { 'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val() }
                    })
                        .then(function (response) {
                            if (response.data.isSuccess) {
                                var cancelText = self.isEdit ? continueEdit : addNewWarehouse;
                                var successText = self.isEdit ? updateSuccess : addSuccess;
                                var body = '';

                                if (self.isEdit) {
                                    if (response.data.errorList.length > 0) {
                                        $.each(response.data.errorList, function (index, val) {
                                            if (body != '') {
                                                body += '<br>';
                                            }
                                            body += val;
                                        });
                                    }
                                }

                                SwalOptions('success', successText, body, done, cancelText, function (result) {
                                    if (result) { // done
                                        //window.location.href = hostName + "/warehouse/";
                                        window.location.href = self.returnURL;
                                    }
                                    else {
                                        // reset
                                        if (!self.isEdit) {
                                            window.location.reload();
                                        }
                                    }
                                });
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
                });
            }
        },
        onSelectLocation: function () {
            var self = this;

            // clear
            google.maps.event.clearListeners(self.gMap, 'click');
            if (self.drawingManager) {
                self.drawingManager.setDrawingMode(null);
                google.maps.event.clearListeners(self.drawingManager, 'overlaycomplete');
            }
            if (self.landCoordinatesPolygon) {
                self.landCoordinatesPolygon.setOptions({ editable: false, draggable: false });
            }

            google.maps.event.addListener(self.gMap, 'click', function (event) {
                self.setLocationMarker(event.latLng, false);
            });
        },
        setLocationMarker: function (latLng, centerMap) {
            var self = this;

            if (self.locationMarker) {
                self.locationMarker.setMap(null);
                self.locationMarker = null;
            }

            self.locationMarker = new google.maps.Marker({
                position: latLng,
                map: self.gMap,
                zIndex: 10000,
                draggable: true
            });

            if (centerMap) {
                self.gMap.setCenter(latLng);
            }
        },
        updateLocation: function () {
            var self = this;
            var latLng = new google.maps.LatLng(self.warehouse.latitude, self.warehouse.longitude);
            self.gMap.setCenter(latLng);
            self.gMap.setZoom(18);
            self.setLocationMarker(latLng, false);
        },
        clearMap: function () {
            var self = this;

            self.warehouse.latitude = '';
            self.warehouse.longitude = '';
            if (self.locationMarker) {
                self.locationMarker.setMap(null);
                self.locationMarker = null;
            }
            google.maps.event.clearListeners(self.gMap, 'click');

            if (self.landCoordinatesPolygon) {
                self.landCoordinatesPolygon.setMap(null);
                self.landCoordinatesPolygon = null;
            }

            self.warehouse.landCoordinates = '';

            if (self.drawingManager) {
                self.drawingManager.setDrawingMode(null);
                google.maps.event.clearListeners(self.drawingManager, 'overlaycomplete');
            }
        },
        onSelectCoordinates: function () {
            var self = this;

            if (!self.landCoordinatesPolygon) {
                self.drawingManager = new google.maps.drawing.DrawingManager({
                    drawingMode: google.maps.drawing.OverlayType.POLYGON,
                    drawingControl: false,
                    polygonOptions: {
                        editable: true,
                        draggable: true,
                    }
                });
                self.drawingManager.setMap(self.gMap);

                google.maps.event.addListener(self.drawingManager, 'overlaycomplete', function (event) {
                    self.drawingManager.setDrawingMode(null);
                    event.overlay.setOptions({ strokeWeight: 1.0, strokeColor: 'gray', fillColor: 'gray' });
                    var path = event.overlay.getPath().getArray().toString();
                    self.landCoordinatesPolygon = event.overlay;
                });
            }
            else {
                self.landCoordinatesPolygon.setOptions({ editable: true, draggable: true });
            }
        },
        saveMap: function () {
            var self = this;

            if (self.locationMarker) {
                self.warehouse.latitude = self.locationMarker.getPosition().lat();
                self.warehouse.longitude = self.locationMarker.getPosition().lng();
            }

            if (self.landCoordinatesPolygon) {
                var path = self.landCoordinatesPolygon.getPath().getArray().toString();
                self.warehouse.landCoordinates = self.getLatLngArray(path);
            }
        },
        setLandCoordinatesPolygon: function () {
            var self = this;
            var path = self.getPath(self.warehouse.landCoordinates);
            self.landCoordinatesPolygon = new google.maps.Polygon({
                paths: path,
                map: self.gMap,
                strokeWeight: 1,
                strokeColor: 'gray',
                fillColor: 'gray',
                editable: true,
                draggable: true
            });
        },
        getLatLngArray: function (path) {
            var polArray = path.split(',');
            var points = [];
            for (var i = 0; i < polArray.length; i += 2) {
                var lat = polArray[i].replace("(", "").replace(")", "");
                var long;
                if (polArray[i + 1]) {
                    long = polArray[i + 1].replace("(", "").replace(")", "");
                }
                points.push(new google.maps.LatLng(lat, long));
            }

            var coordinates = JSON.stringify(points, null, 2);
            return coordinates.replace(/lng/g, "x").replace(/lat/g, "y");
        },
        getPath: function (coordinates) {
            return JSON.parse(coordinates.replace(/x/g, "lng").replace(/y/g, "lat"));
        },
        closeMap: function () {
            var self = this;
            google.maps.event.clearListeners(self.gMap, 'click');
            if (self.drawingManager) {
                google.maps.event.clearListeners(self.drawingManager, 'overlaycomplete');
                self.drawingManager.setMap(null);
                self.drawingManager = null;
            }
        }
    }
});
