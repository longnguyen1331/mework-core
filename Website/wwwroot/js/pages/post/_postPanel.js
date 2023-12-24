(function ($) {
    'use strict';
    $(document).on('ready', function () {
        loadPostPanel(0);
    });
})(jQuery); // End jQuery

function prevPage(page) {
    if ($('.post-prev').hasClass('disabled'))
        return;

    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadPostPanel(skip);
}

function goToPage(page) {
    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadPostPanel(skip);
}

function nextPage(page) {
    if ($('.post-next').hasClass('disabled'))
        return;

    let currentPage = '#page-' + page;
    $('.pagination .active').removeClass('active');
    $(currentPage).addClass('active');

    let skip = (page - 1) * take;

    loadPostPanel(skip);
}

function onSearch(url) {
    window.location.href = url + '?keyword=' + $('#search-keyword').val();
}

function loadPostPanel(skip) {
    let data = {
        skip: skip,
        take: take,
        filterText: $('#search-keyword').val(),
        postCategoryId: postCategoryId != '' ? postCategoryId : null,
        tags: tags != '' ? tags : null
    }

    $.ajax({
        url: '/postCategory/GetPostsPanel',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'html',
        contentType: 'application/json',
        success: function (data) {
            if (data) {
                $('#post-panel').html(data);
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