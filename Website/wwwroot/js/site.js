// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$('#header').load('/Shared/Header?currentURL=' + window.location.pathname, function () {
//    $.getScript("/js/bootsnav.js");
//});

const take = 10;

function onShowModal(element) {
    $(element).modal();
}

function onHideModal(element) {
    $(element).modal('hide');
}

function toastNoti(type, title, msg) {
    switch (type) {
        case "info":
            toastr.info(msg, title);
            break;
            
        case "success":
            toastr.success(msg, title);
            break;
            
        case "warning":
            toastr.warning(msg, title);
            break;
            
        case "error":
            toastr.error(msg, title);
            break;
    }
}

function getFirstDayOfWeek(d) {
    // 👇️ clone date object, so we don't mutate it
    const date = new Date(d);
    const day = date.getDay(); // 👉️ get day of week

    // 👇️ day of month - day of week (-6 if Sunday), otherwise +1
    const diff = date.getDate() - day + (day === 0 ? -6 : 1);

    return new Date(date.setDate(diff));
}