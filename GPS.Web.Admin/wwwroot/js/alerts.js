
function SwalAlertOk(type, title, text, callback) {
    Swal.mixin({
        toast: false,
        showConfirmButton: true,
        animation: false
    }).fire({
        type: type,
        title: title,
        text: text
    }).then((result) => {
        if (callback) {
            callback();
        }
    });
}

function SwalAlert(type, title, text) {
    Swal.mixin({
        toast: false,
        showConfirmButton: true,
        animation: false
    }).fire({
        type: type,
        title: title,
        html: text,
    });
}


function SwalConfirm(type, title, text, Yes, Cancel, callback) {
    swal.fire({
        title: title,
        text: text,
        type: type,
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#d33',
        confirmButtonText: Yes,
        cancelButtonText: Cancel,
        animation: false,
    }).then((result) => {
        callback(result.value);
    });
}

function SwalOptions(type, title, text, Done, Cancel, callback) {
    swal.fire({
        title: title,
        html: text,
        type: type,
        showCancelButton: true,
        cancelButtonColor: '#28a745',
        confirmButtonText: Done,
        cancelButtonText: Cancel,
        animation: true,
    }).then((result) => {
        callback(result.value);
    });
}

function SwalFlipOptions(type, title, text, Done, Cancel, callback) {
    swal.fire({
        title: title,
        html: text,
        type: type,
        showCancelButton: true,
        cancelButtonColor: '#28a745',
        confirmButtonText: Done,
        cancelButtonText: Cancel,
        animation: true,
        showClass: {
            popup: 'animate__animated animate__flipInX'
        },
    }).then((result) => {
        callback(result.value);
    });
}

function SwalFlipAlertOk(type, title, text, callback) {
    Swal.mixin({
        toast: false,
        showConfirmButton: true,
        animation: true,
        showClass: {
            popup: 'animate__animated animate__flipInX'
        },
    }).fire({
        type: type,
        title: title,
        text: text
    }).then((result) => {
        if (callback) {
            callback(result.value);
        }
    });
}