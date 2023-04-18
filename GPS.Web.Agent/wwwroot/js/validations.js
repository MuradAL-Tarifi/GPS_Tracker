// NumberValidate
function NumberValidate(source, arguments) {
    if (isNaN(arguments.Value))
        arguments.IsValid = false;
    else if (arguments.Value < 0)
        arguments.IsValid = false;
    else if (!(arguments.Value.indexOf('.') === -1))
        arguments.IsValid = false;
    else
        arguments.IsValid = true;
}

// IntegerNumberValidate
function IntegerNumberValidate(source, arguments) {
    if (isNaN(arguments.Value))
        arguments.IsValid = false;
    else if (arguments.Value <= 0)
        arguments.IsValid = false;
    else if (!(String(~~Number(arguments.Value)) === arguments.Value && ~~Number(arguments.Value) >= 0))
        arguments.IsValid = false;
    //else if (arguments.Value.length > 2)
    //    arguments.IsValid = false;
    else
        arguments.IsValid = true;
}

// IntegerNumberValidate with zero
function WZ_IntegerNumberValidate(source, arguments) {
    if (isNaN(arguments.Value))
        arguments.IsValid = false;
    else if (arguments.Value < 0)
        arguments.IsValid = false;
    else if (!(String(~~Number(arguments.Value)) === arguments.Value && ~~Number(arguments.Value) >= 0))
        arguments.IsValid = false;
    //else if (arguments.Value.length > 2)
    //    arguments.IsValid = false;
    else
        arguments.IsValid = true;
}

function ArOnly(event) {
    var x = event.key;
    var pattern = /[\u0600-\u06FF]/;
    var multicharacter = ["Backspace", "Delete"];
    if (multicharacter.indexOf(x) > -1) {
        return true;
    }

    if (pattern.test(x)) {
        return true;
    }
    else {
        return false;
    }
}

function En(event) {
    var x = event.key;
    var pattern = /[\u0600-\u06FF]/;
    var multicharacter = ["Home", "CAPS LOCK", "ALT", "CTRL", "META", "Tab", "Shift", "Control", "Backspace", "ArrowLeft", "ArrowRight", " ", "-", "F5", "Delete", "End", "(", ")", "'", "'", "\"", "\"", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    if (multicharacter.indexOf(x) > -1) {
        return true;
    }

    if (pattern.test(x)) {
        return false;
    }
    else {
        return true;
    }
}