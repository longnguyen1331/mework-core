(function ($) {
    'use strict';
    $(document).on('ready', function () {
        $('.top-banner-carousel').owlCarousel({
            loop: true,
            nav: false,
            dots: false,
            autoplay: true,
            items: 1,
            navText: [
                "<i class='fa fa-angle-left'></i>",
                "<i class='fa fa-angle-right'></i>"
            ],
            animateIn: 'fadeIn',
            animateOut: 'fadeOut'
        });

 
    

        $('.highlight-services-items-carousel').owlCarousel({
            loop: true,
            nav: false,
            dots: true,
            autoplay: true,
            margin: 20,
            responsive: {
                0: {
                    items: 1
                },
                425: {
                    items: 3
                },
                960: {
                    items: 1
                }
            },
            navText: [
                "<i class='fa fa-angle-left'></i>",
                "<i class='fa fa-angle-right'></i>"
            ],
        });

        $('.custom_prevService').on('click', function (e) {
            e.preventDefault();

            $('.highlight-services-items-carousel').trigger('prev.owl.carousel');
        });

        $('.custom_nextService').on('click', function (e) {
            e.preventDefault();

            $('.highlight-services-items-carousel').trigger('next.owl.carousel');
        });

        $('.post-related-items-carousel').owlCarousel({
            loop: false,
            nav: false,
            dots: true,
            autoplay: true,
            margin: 20,
            responsive: {
                0: {
                    items: 1
                },
                765: {
                    items: 2
                }
            },
            navText: [
                "<i class='fa fa-angle-left'></i>",
                "<i class='fa fa-angle-right'></i>"
            ],
        });

    });

    if ($('#hot-news'))
        loadHotPost(0, 5);
    if ($('#tag-panel'))
        loadTagsOfPost(0, 15);
})(jQuery); // End jQuery

function loadHotPost(skip, take) {
    let data = {
        skip: skip,
        take: take,
        isTopViews: true
    }

    $.ajax({
        url: '/postCategory/getPostsLeftPanel',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'html',
        contentType: 'application/json',
        success: function (data) {
            console.log(data);
            if (data) {
                $('#hot-news').html(data);
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

function loadTagsOfPost(skip, take) {
    $.ajax({
        url: '/postCategory/getTagsPanel?skip=' + skip + '&take=' + take,
        type: 'GET',
        dataType: 'html',
        contentType: 'application/json',
        success: function (data) {
            console.log(data);
            if (data) {
                $('#tag-panel').html(data);
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