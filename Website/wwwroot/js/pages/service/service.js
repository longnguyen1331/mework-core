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
    });
})(jQuery); // End jQuery
