function colorByState(state) {
    switch (state) {
        case states.moving:
            return "green";
        case states.stopped:
            return "red";
        case states.idle:
            return "yellow";
        default:
            return "gray";
    }
}

function bearing(from, to) {
    // Convert to radians.
    var lat1 = from.latRadians();
    var lon1 = from.lngRadians();
    var lat2 = to.latRadians();
    var lon2 = to.lngRadians();
    // Compute the angle.
    var angle = - Math.atan2(Math.sin(lon1 - lon2) * Math.cos(lat2), Math.cos(lat1) * Math.sin(lat2) - Math.sin(lat1) * Math.cos(lat2) * Math.cos(lon1 - lon2));
    if (angle < 0.0)
        angle += Math.PI * 2.0;
    if (angle == 0) { angle = 1.5; }
    return angle;
}


function bearingBetweenLocations(Latitude1, Longitude1, Latitude2, Longitude2) {

    var PI = 3.14159;
    var lat1 = Latitude1 * PI / 180;
    var long1 = Longitude1 * PI / 180;
    var lat2 = Latitude2 * PI / 180;
    var long2 = Longitude2 * PI / 180;
    var dLon = (long2 - long1);
    var y = Math.sin(dLon) * Math.cos(lat2);
    var x = (Math.cos(lat1) * Math.sin(lat2) - (Math.sin(lat1) * Math.cos(lat2) * Math.cos(dLon)));
    var brng = Math.atan2(y, x);
    brng = brng * (180 / Math.PI);
    brng = (brng + 360) % 360;
    return brng;
}


var canvas;
function drawRotated(src, degrees) {
    //if (canvas) document.body.removeChild(canvas);

    var image = document.createElement("img");
    image.src = src;

    canvas = document.createElement("canvas");
    var ctx = canvas.getContext("2d");
    canvas.style.width = "20%";

    if (degrees == 90 || degrees == 270) {
        canvas.width = image.height;
        canvas.height = image.width;
    } else {
        canvas.width = image.width;
        canvas.height = image.height;
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    if (degrees == 90 || degrees == 270) {
        ctx.translate(image.height / 2, image.width / 2);
    } else {
        ctx.translate(image.width / 2, image.height / 2);
    }
    ctx.rotate(degrees * Math.PI / 180);
    ctx.drawImage(image, -image.width / 2, -image.height / 2);

    return canvas.toDataURL('image/png');
    //document.body.appendChild(canvas);
}

function rotateIcons(icons) {

    $.each(icons, function (index, icon) {
        $("img[src='" + icon.url + "']").css(
            {
                '-webkit-transform': 'rotate(' + icon.degree + 'deg)',
                '-moz-transform': 'rotate(' + icon.degree + 'deg)',
                '-ms-transform': 'rotate(' + icon.degree + 'deg)',
                'transform': 'rotate(' + icon.degree + 'deg)'
            });
    });
}

function isArray(value) {
    return value && typeof value === 'object' && value.constructor === Array;
}

function randDarkColor() {
    var lum = -0.25;
    var hex = String('#' + Math.random().toString(16).slice(2, 8).toUpperCase()).replace(/[^0-9a-f]/gi, '');
    if (hex.length < 6) {
        hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
    }
    var rgb = "#",
        c, i;
    for (i = 0; i < 3; i++) {
        c = parseInt(hex.substr(i * 2, 2), 16);
        c = Math.round(Math.min(Math.max(0, c + (c * lum)), 255)).toString(16);
        rgb += ("00" + c).substr(c.length);
    }
    return rgb;
}

function DatesDifference(date1, date2) {
    //Get 1 day in milliseconds
    var one_day = 1000 * 60 * 60 * 24;

    // Convert both dates to milliseconds
    var date1_ms = new Date(Date.parse(date1)).getTime();
    var date2_ms = new Date(Date.parse(date2)).getTime();

    // Calculate the difference in milliseconds
    var difference_ms = date2_ms - date1_ms;
    //take out milliseconds
    difference_ms = difference_ms / 1000;
    var seconds = Math.floor(difference_ms % 60);
    difference_ms = difference_ms / 60;
    var minutes = Math.floor(difference_ms % 60);
    difference_ms = difference_ms / 60;
    var hours = Math.floor(difference_ms % 24);
    var days = Math.floor(difference_ms / 24);

    var secondsS = seconds.toString().length == 1 ? "0" + seconds : seconds;
    var minutesS = minutes.toString().length == 1 ? "0" + minutes : minutes;
    var hoursS = hours.toString().length == 1 ? "0" + hours : hours;

    var result = hoursS + ":" + minutesS + ":" + secondsS;
    return result;
}


function GetLatLngArray(path) {
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
    return points;
}

function IsPointInPolygon(poly, point) {
    var i, j;
    var c = false;
    for (i = 0, j = poly.length - 1; i < poly.length; j = i++) {
        if ((((poly[i].lat() <= point.lat()) && (point.lat() < poly[j].lat()))
            || ((poly[j].lat() <= point.lat()) && (point.lat() < poly[i].lat())))
            && (point.lng() < (poly[j].lng() - poly[i].lng()) * (point.lat() - poly[i].lat())
                / (poly[j].lat() - poly[i].lat()) + poly[i].lng())) {

            c = !c;
        }
    }

    return c;
}