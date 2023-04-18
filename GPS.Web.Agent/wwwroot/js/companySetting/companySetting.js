$(document).ajaxStart(function () { Pace.restart(); });

$(document).ready(function () {
    $('#logoInput').change(function (e) {
        var isValidFileImage = fileSizeAndExtenionValidation(this, 50);
        if (isValidFileImage) {
            //document.getElementById('containerInputSize').classList.remove("d-none");
            document.getElementById('logoInputSize').innerHTML = `${fileSize(this)}`;
            var outPutLogo = document.getElementById('outPutLogo');
            outPutLogo.classList.remove("d-none");
            outPutLogo.src = URL.createObjectURL(this.files[0]);
            outPutLogo.onload = function () {
                URL.revokeObjectURL(outPutLogo.src);
            }
        }
    });
    $("#AddUpdateForm").on("submit", function (event) {
        event.preventDefault();
        // validate
        var validator = $("#AddUpdateForm").validate();
        if ($("#AddUpdateForm").valid()) {
            var ajaxConfig = {
                url: hostName + "/CompanySetting/AddUpdate",
                type: $(this).attr("method"),
                dataType: "JSON",
                data: new FormData(this),
                processData: false,
                contentType: false,
                success: function (response) {
                    //alert(response);
                    if (!response.data) {
                        SwalAlert("error", "حدث خطأ في النظام", response.data.errorList[0]);
                    } else {
                        SwalFlipAlertOk('success', saveSuccess, '', function () {
                            window.location.reload();
                        });
                    }
                },
                error: function (xhr, desc, error) {
                    if (error.response.data.errorList) {
                        SwalAlert("error", "", error.response.data.errorList[0]);
                    }
                    else {
                        SwalAlert("error", "حدث خطأ في النظام", "");
                    }
                }
            };
            $.ajax(ajaxConfig);
        }
        else {
            validator.focusInvalid();
        }
        return false;
    });
});

function fileSizeAndExtenionValidation(oInput, maxSizejpeg) {
    var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".png"];
    var isValid = false;
    if (oInput.type == "file") {
        var fileName = oInput.value;
        if (fileName.length > 0)
            for (var i = 0; i < _validFileExtensions.length; i++) {
                var currentExtenion = _validFileExtensions[i];
                if (fileName.substr(fileName.length - currentExtenion.length, currentExtenion.length)
                    .toLowerCase() == currentExtenion.toLowerCase()) {
                    isValid = true;
                }
            }
            if (!isValid) {
                alert("Sorry, this file is invalid, allowed extensions are: " + _validFileExtensions.toString());
                oInput.value = "";
                isValid = false;
            }
           if (oInput.files && oInput.files[0]) {
             var fsize = oInput.files[0].size / 1024 ;
            if (fsize > maxSizejpeg) {
                alert('This file size is: ' + fsize.toFixed(2) +
                    "KB. Files should not exceed" + (maxSizejpeg) + " KB ");
                oInput.value = "";
                isValid = false;
            } else {
                isValid = true;
            }
        }
        return isValid;
    }
    oInput.value = "";
    return false;
 }
var fileSize = function (oInput) {
    var _size = oInput.files[0].size;
    var fSExt = new Array('Bytes', 'KB', 'MB', 'GB'),
        i = 0; while (_size > 900) { _size /= 1024; i++; }
    return (Math.round(_size * 100) / 100) + ' ' + fSExt[i];
}





