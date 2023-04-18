
$(document).ready(function () {
    $.get(hostName + '/Account/GetSystemSettings').done(function (response) {
        var data = JSON.parse(response);
        if (data.LogoFileBase64.length > 0) {
            var imgSrc = `data:image/png;base64,${data.LogoFileBase64}`;
            $('.img-fluid').attr('src', imgSrc);
        }
    });
});




