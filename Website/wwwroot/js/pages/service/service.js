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

        $('.doctor-items-carousel').owlCarousel({
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

        $('.custom_prevSpecialty').on('click', function (e) {
            e.preventDefault();

            $('.doctor-items-carousel').trigger('prev.owl.carousel');
        });

        $('.custom_nextSpecialty').on('click', function (e) {
            e.preventDefault();

            $('.doctor-items-carousel').trigger('next.owl.carousel');
        });

        $('.service-related-items-carousel').owlCarousel({
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
                },
                1080: {
                    items: 3
                }
            },
            navText: [
                "<i class='fa fa-angle-left'></i>",
                "<i class='fa fa-angle-right'></i>"
            ],
        });
    });
})(jQuery); // End jQuery
