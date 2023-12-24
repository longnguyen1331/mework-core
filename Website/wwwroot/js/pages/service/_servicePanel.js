(function ($) {
    'use strict';
    $(document).on('ready', function () {
        loadServicePanel(0);
    });
})(jQuery); // End jQuery

function prevPage(page) {
    if ($('.post-prev').hasClass('disabled'))
        return;

    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadServicePanel(skip);
}

function goToPage(page) {
    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadServicePanel(skip);
}

function nextPage(page) {
    if ($('.post-next').hasClass('disabled'))
        return;

    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadServicePanel(skip);
}

function onSearch(url) {
    window.location.href = url + '?keyword=' + $('#search-keyword').val();
}

function loadServicePanel(skip) {
    let data = {
        skip: skip,
        take: take,
        filterText: $('#search-keyword').val(),
        serviceTypeId: serviceTypeId != '' ? serviceTypeId : null
    }

    $.ajax({
        url: '/service/getServicesPanel',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'html',
        contentType: 'application/json',
        success: function (data) {
            if (data) {
                $('#service-panel').html(data);
            }
        },
        failure: function (response) {
            toastNoti('error', '', response.responseText);
        },
        error: function (response) {
            toastNoti('error', '', response.responseText);
        }
    });
}