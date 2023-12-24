function bookingServicePrevPage(page) {
    if ($('.post-prev').hasClass('disabled'))
        return;

    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadBookingServicePanel(skip);
}

function bookingServiceGoToPage(page) {
    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadBookingServicePanel(skip);
}

function bookingServiceNextPage(page) {
    if ($('.post-next').hasClass('disabled'))
        return;

    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadBookingServicePanel(skip);
}

function loadBookingServicePanel(skip) {
    let data = {
        skip: skip,
        take: take,
        filterText: $('#service-search-keyword').val(),
        doctorId: doctorId != null && doctorId != '' ? doctorId : null
    }

    $.ajax({
        url: '/service/getServicesPanelForBookingAppointment',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'html',
        contentType: 'application/json',
        success: function (data) {
            if (data) {
                $('#loadBookingAppointment').append(data);
            }
            $('#loadBookingAppointment').find('div.modal-header').slideDown('fast');
            $('#loadBookingAppointment').find('div.modal-body').slideDown('slow');
            $('#loadBookingAppointment').find('div.modal-footer').slideDown('slow');
        },
        failure: function (response) {
            toastNoti('error', '', response.responseText);
        },
        error: function (response) {
            toastNoti('error', '', response.responseText);
        }
    });
}