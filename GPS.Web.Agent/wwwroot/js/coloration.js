const DeviceTypes = {
    T1: 1,
    Omatix_G08: 2,
    T355: 3,
    Eco_4: 4,
    T366: 5,
    MVT800: 6,
    Teltonika_FM1120: 7,
    Teltonika_FMA120: 8,
    Teltonika_FMM130: 9,
    Teltonika_FMM640: 10
}

function ExpRowBgClass(date) {
    var dateNow = new Date();
    var diffrence = Date.parse(date) - Date.parse(dateNow);
    var days = parseInt((diffrence) / (24 * 3600 * 1000));

    if (days > 30) {
        return "row-success";
    }
    else if (days >= 0 && days <= 30) {
        return "row-warning";
    }
    return "row-danger";
}

function ExpBgClass(date) {
    var dateNow = new Date();
    var diffrence = Date.parse(date) - Date.parse(dateNow);
    var days = parseInt((diffrence) / (24 * 3600 * 1000));

    if (days > 30) {
        return "badge-success";
    }
    else if (days >= 0 && days <= 30) {
        return "badge-warning";
    }
    return "badge-danger";
}

function DeviceTypeBgClass(DeviceTypeId) {
    switch (DeviceTypeId) {
        case DeviceTypes.T1:
            return "t1-bg";
        case DeviceTypes.Omatix_G08:
            return "omatix_G08-bg";
        case DeviceTypes.T355:
            return "t355-bg";
        case DeviceTypes.Eco_4:
            return "eco_4-bg";
        case DeviceTypes.T366:
            return "t366-bg";
        case DeviceTypes.MVT800:
            return "mVT800-bg";
        case DeviceTypes.Teltonika_FM1120:
            return "teltonika_FM1120-bg";
        case DeviceTypes.Teltonika_FMA120:
            return "teltonika_FMA120-bg";
        case DeviceTypes.Teltonika_FMM130:
            return "teltonika_FMM130-bg";
        case DeviceTypes.Teltonika_FMM640:
            return "teltonika_FMM640-bg";
        default:
            return "";
    }
}
