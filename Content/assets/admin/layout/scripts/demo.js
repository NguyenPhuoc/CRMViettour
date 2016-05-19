/**
Demo script to handle the theme demo
**/
var Demo = function() {

    // Handle Theme Settings
    var handleTheme = function() {

        var panel = $('.theme-panel');

        if ($('body').hasClass('page-boxed') === false) {
            $('.layout-option', panel).val("fluid");
        }

        $('.sidebar-option', panel).val("default");
        $('.page-header-option', panel).val("fixed");
        $('.page-footer-option', panel).val("default");
        if ($('.sidebar-pos-option').attr("disabled") === false) {
            $('.sidebar-pos-option', panel).val(Metronic.isRTL() ? 'right' : 'left');
        }

        //handle theme layout
        var resetLayout = function() {
            $("body").
            removeClass("page-boxed").
            removeClass("page-footer-fixed").
            removeClass("page-sidebar-fixed").
            removeClass("page-header-fixed").
            removeClass("page-sidebar-reversed");

            $('.page-header > .page-header-inner').removeClass("container");

            if ($('.page-container').parent(".container").size() === 1) {
                $('.page-container').insertAfter('body > .clearfix');
            }

            if ($('.page-footer > .container').size() === 1) {
                $('.page-footer').html($('.page-footer > .container').html());
            } else if ($('.page-footer').parent(".container").size() === 1) {
                $('.page-footer').insertAfter('.page-container');
                $('.scroll-to-top').insertAfter('.page-footer');
            }

             $(".top-menu > .navbar-nav > li.dropdown").removeClass("dropdown-dark");

            $('body > .container').remove();
        };

        var lastSelectedLayout = '';

        var setLayout = function() {

            var layoutOption = $('.layout-option', panel).val();
            var sidebarOption = $('.sidebar-option', panel).val();
            var headerOption = $('.page-header-option', panel).val();
            var footerOption = $('.page-footer-option', panel).val();
            var sidebarPosOption = $('.sidebar-pos-option', panel).val();
            var sidebarStyleOption = $('.sidebar-style-option', panel).val();
            var sidebarMenuOption = $('.sidebar-menu-option', panel).val();
            var headerTopDropdownStyle = $('.page-header-top-dropdown-style-option', panel).val();

            if (sidebarOption == "fixed" && headerOption == "default") {
                alert('Default Header with Fixed Sidebar option is not supported. Proceed with Fixed Header with Fixed Sidebar.');
                $('.page-header-option', panel).val("fixed");
                $('.sidebar-option', panel).val("fixed");
                sidebarOption = 'fixed';
                headerOption = 'fixed';
            }

            resetLayout(); // reset layout to default state

            if (layoutOption === "boxed") {
                $("body").addClass("page-boxed");

                // set header
                $('.page-header > .page-header-inner').addClass("container");
                var cont = $('body > .clearfix').after('<div class="container"></div>');

                // set content
                $('.page-container').appendTo('body > .container');

                // set footer
                if (footerOption === 'fixed') {
                    $('.page-footer').html('<div class="container">' + $('.page-footer').html() + '</div>');
                } else {
                    $('.page-footer').appendTo('body > .container');
                }
            }

            if (lastSelectedLayout != layoutOption) {
                //layout changed, run responsive handler: 
                Metronic.runResizeHandlers();
            }
            lastSelectedLayout = layoutOption;

            //header
            if (headerOption === 'fixed') {
                $("body").addClass("page-header-fixed");
                $(".page-header").removeClass("navbar-static-top").addClass("navbar-fixed-top");
            } else {
                $("body").removeClass("page-header-fixed");
                $(".page-header").removeClass("navbar-fixed-top").addClass("navbar-static-top");
            }

            //sidebar
            if ($('body').hasClass('page-full-width') === false) {
                if (sidebarOption === 'fixed') {
                    $("body").addClass("page-sidebar-fixed");
                    $("page-sidebar-menu").addClass("page-sidebar-menu-fixed");
                    $("page-sidebar-menu").removeClass("page-sidebar-menu-default");
                    Layout.initFixedSidebarHoverEffect();
                } else {
                    $("body").removeClass("page-sidebar-fixed");
                    $("page-sidebar-menu").addClass("page-sidebar-menu-default");
                    $("page-sidebar-menu").removeClass("page-sidebar-menu-fixed");
                    $('.page-sidebar-menu').unbind('mouseenter').unbind('mouseleave');
                }
            }

            // top dropdown style
            if (headerTopDropdownStyle === 'dark') {
                $(".top-menu > .navbar-nav > li.dropdown").addClass("dropdown-dark");
            } else {
                $(".top-menu > .navbar-nav > li.dropdown").removeClass("dropdown-dark");
            }

            //footer 
            if (footerOption === 'fixed') {
                $("body").addClass("page-footer-fixed");
            } else {
                $("body").removeClass("page-footer-fixed");
            }

            //sidebar style
            if (sidebarStyleOption === 'light') {
                $(".page-sidebar-menu").addClass("page-sidebar-menu-light");
            } else {
                $(".page-sidebar-menu").removeClass("page-sidebar-menu-light");
            }

            //sidebar menu 
            if (sidebarMenuOption === 'hover') {
                if (sidebarOption == 'fixed') {
                    $('.sidebar-menu-option', panel).val("accordion");
                    alert("Hover Sidebar Menu is not compatible with Fixed Sidebar Mode. Select Default Sidebar Mode Instead.");
                } else {
                    $(".page-sidebar-menu").addClass("page-sidebar-menu-hover-submenu");
                }
            } else {
                $(".page-sidebar-menu").removeClass("page-sidebar-menu-hover-submenu");
            }

            //sidebar position
            if (Metronic.isRTL()) {
                if (sidebarPosOption === 'left') {
                    $("body").addClass("page-sidebar-reversed");
                    $('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'right'
                    });
                } else {
                    $("body").removeClass("page-sidebar-reversed");
                    $('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'left'
                    });
                }
            } else {
                if (sidebarPosOption === 'right') {
                    $("body").addClass("page-sidebar-reversed");
                    $('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'left'
                    });
                } else {
                    $("body").removeClass("page-sidebar-reversed");
                    $('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'right'
                    });
                }
            }

            Layout.fixContentHeight(); // fix content height            
            Layout.initFixedSidebar(); // reinitialize fixed sidebar
        };

        // handle theme colors
        var setColor = function(color) {
            var color_ = (Metronic.isRTL() ? color + '-rtl' : color);
            $('#style_color').attr("href", Layout.getLayoutCssPath() + 'themes/' + color_ + ".css");
            if (color == 'light2') {
                $('.page-logo img').attr('src', Layout.getLayoutImgPath() + 'logo-invert.png');
            } else {
                $('.page-logo img').attr('src', Layout.getLayoutImgPath() + 'logo.png');
            }
        };

        $('.toggler', panel).click(function() {
            $('.toggler').hide();
            $('.toggler-close').show();
            $('.theme-panel > .theme-options').show();
        });

        $('.toggler-close', panel).click(function() {
            $('.toggler').show();
            $('.toggler-close').hide();
            $('.theme-panel > .theme-options').hide();
        });

        $('.theme-colors > ul > li', panel).click(function() {
            var color = $(this).attr("data-style");
            setColor(color);
            $('ul > li', panel).removeClass("current");
            $(this).addClass("current");
        });

        // set default theme options:

        if ($("body").hasClass("page-boxed")) {
            $('.layout-option', panel).val("boxed");
        }

        if ($("body").hasClass("page-sidebar-fixed")) {
            $('.sidebar-option', panel).val("fixed");
        }

        if ($("body").hasClass("page-header-fixed")) {
            $('.page-header-option', panel).val("fixed");
        }

        if ($("body").hasClass("page-footer-fixed")) {
            $('.page-footer-option', panel).val("fixed");
        }

        if ($("body").hasClass("page-sidebar-reversed")) {
            $('.sidebar-pos-option', panel).val("right");
        }

        if ($(".page-sidebar-menu").hasClass("page-sidebar-menu-light")) {
            $('.sidebar-style-option', panel).val("light");
        }

        if ($(".page-sidebar-menu").hasClass("page-sidebar-menu-hover-submenu")) {
            $('.sidebar-menu-option', panel).val("hover");
        }

        var sidebarOption = $('.sidebar-option', panel).val();
        var headerOption = $('.page-header-option', panel).val();
        var footerOption = $('.page-footer-option', panel).val();
        var sidebarPosOption = $('.sidebar-pos-option', panel).val();
        var sidebarStyleOption = $('.sidebar-style-option', panel).val();
        var sidebarMenuOption = $('.sidebar-menu-option', panel).val();

        $('.layout-option, .page-header-option, .page-header-top-dropdown-style-option, .sidebar-option, .page-footer-option, .sidebar-pos-option, .sidebar-style-option, .sidebar-menu-option', panel).change(setLayout);
    };

    // handle theme style
    var setThemeStyle = function(style) {
        var file = (style === 'rounded' ? 'components-rounded' : 'components');
        file = (Metronic.isRTL() ? file + '-rtl' : file);

        $('#style_components').attr("href", Metronic.getGlobalCssPath() + file + ".css");

        if ($.cookie) {
            $.cookie('layout-style-option', style);
        }
    };

    // Handle 
    var handlePromo = function() {

        var init = function() {
            var html = '';

            html  = '<div class="promo-layer" style="z-index: 100000; position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0, 0.8)">';
            html += '   <div style="z-index: 100001; top: 50%; left: 50%; margin: -300px 0 0 -400px; width: 800px; height: 600px; position: fixed;">';
            html += '       <div class="row">';
            html += '           <div class="col-md-12" style="text-align: center">';
            html += '               <h3 style="color: white; margin-bottom: 30px; font-size: 28px; line-height: 36px; font-weight: 400;">You are one step behind in choosing a perfect <br>admin theme for your project.</h3>';
            html += '               <p style="color: white; font-size: 18px;">Just to recap some important facts about Metronic:</p>';
            html += '               <ul style="list-style:none; margin: 30px auto 20px auto; padding: 10px; display: block; width: 550px;  text-align: left; background: #fddf00;  color: #000000;transform:rotate(-2deg);">';
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      The Most Popular #1 Selling Admin Theme of All Time.';
            html += '                   </li>';
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      Trusted By Over 26000 Users Around The Globe.';
            html += '                   </li>';  
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      Used By Listed Companies In Small To Enterprise Solutions.';
            html += '                   </li>';  
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      Includes 500+ Templates, 80+ Plugins, 1000+ UI Components.';
            html += '                   </li>';  
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      Backed By A Team With Combined 32 Years of Experience In The Field.';
            html += '                   </li>';  
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      A Product Of Over 2 Years Of Continuous Improvements';
            html += '                   </li>'; 
            html += '                   <li style="list-style:none; padding: 4px 8px; font-size: 15px;">';
            html += '                      <span style="display: inline-block; width: 10px; height: 10px; border-radius: 20px !important; background: rgba(0, 0, 0, 0.2); margin-right: 5px;  margin-top: 7px;"></span>';
            html += '                      Get All The Above & Even More Just For 27$';
            html += '                   </li>'; 
            html += '               </ul>';
            html += '           </div>';
            html += '       </div>';
            html += '       <div class="row">';
            html += '           <div class="col-md-12" style="margin-top: 20px;">';
            html += '               <center><a class="btn btn-circle btn-danger btn-lg" style="padding: 12px 28px; font-size: 14px; text-transform: uppercase1;" href="../../../../../../themeforest.net/item/metronic-responsive-admin-dashboard-template/4021469@ref=keenthemes&utm_source=preview&utm_medium=banner&utm_campaign=Preview%20Engage" title="Purchase Metronic just for 27$ and get lifetime updates for free" target="_blank">Purchase Now!</a>';
            html += '               &nbsp;&nbsp;<a class="btn btn-circle btn-default btn-lg promo-remind" style="padding: 11px 28px; font-size: 14px; text-transform: uppercase1;background: none; color: #fff;" href="javascript:;">Remind Me Later</a>';
            html += '               <a class="btn btn-circle btn-default btn-lg promo-dismiss" style="padding: 12px 12px; font-size: 14px; text-transform: uppercase1; background: none; color: #aaa; border: 0" href="javascript:;">Dismiss</a></center>';
            html += '           </div>';
            html += '       </div>';
            html += '   </div>';
            html += '</div>';

            $('body').append(html);

            $('.promo-dismiss').click(function(){
                $('.promo-layer').remove();

                $.cookie('user-dismiss', 1, { expires: 7, path: '../../../../../default.htm' });
            });

            $('.promo-remind').click(function(){
                $('.promo-layer').remove();

                $.cookie('user-page-views', 1, { expires: 1, path: '../../../../../default.htm' });
            });
        }

        if ($.cookie) {
            var pageViews = $.cookie('user-page-views') ? parseInt($.cookie('user-page-views')) : 0;
            var userDismiss = $.cookie('user-dismiss') ? parseInt($.cookie('user-dismiss')) : 0;
            
            pageViews = pageViews + 1;
            $.cookie('user-page-views', pageViews, { expires: 1, path: '../../../../../default.htm' });

            //alert(pageViews);

            if (userDismiss === 0 && (pageViews === 10 || pageViews === 30 || pageViews === 50)) {
                setTimeout(init, 1000);
            }
        } else {
            return;
        }
    };

    return {

        //main function to initiate the theme
        init: function() {
            // handles style customer tool
            handleTheme(); 

            handlePromo();
            
            // handle layout style change
            $('.theme-panel .layout-style-option').change(function() {
                 setThemeStyle($(this).val());
            });

            // set layout style from cookie
            if ($.cookie && $.cookie('layout-style-option') === 'rounded') {
                setThemeStyle($.cookie('layout-style-option'));
                $('.theme-panel .layout-style-option').val($.cookie('layout-style-option'));
            }            
        }
    };

}();