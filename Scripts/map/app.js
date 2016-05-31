; (function ($, window, undefined) {
    'use strict';

    var $doc = $(document),
        Modernizr = window.Modernizr,
        lt_ie9 = false;

    $(document).ready(function () {

        $.fn.foundationMediaQueryViewer ? $doc.foundationMediaQueryViewer() : null;
        $.fn.foundationTabs ? $doc.foundationTabs() : null;

        if (Modernizr.touch && !window.location.hash) {
            $(window).load(function () {
                setTimeout(function () {
                    window.scrollTo(0, 1);
                }, 0);
            });
        }

        $("body").iealert({
            support: 'ie7'
        });

        prettyPrint();

        if ($('html').hasClass('lt-ie9')) {
            lt_ie9 = true;
        }

        //Just the map
        var simple = new Maplace();

        $('#menuMap').one('inview', function (event, isInView) {
            if (isInView) {
                new Maplace({map_div: '#map',controls_type: 'list',controls_cssclass: 'side-nav',controls_on_map: false,locations: addressList}).Load();
            }
        });

        if (lt_ie9) {
            new Maplace({
                map_div: '#map',
                controls_type: 'list',
                controls_cssclass: 'side-nav',
                controls_on_map: false,
                locations: addressList
            }).Load();
        }

    });//ready

})(jQuery, this);
