
$(document).ready(wrapResize);
$(window).resize(wrapResize);

function wrapResize() {
    var nav = $('#navbar');
    var navApp = $('#navbarApplication');
    var topApp = 0;
    var offsetApp = 0;
    if (navApp) {
        topApp = navApp.height();
        if (navApp.offset())
         offsetApp = navApp.offset().top;
    }
    var wrap = $('.full-height');
    var bottom2 = $('#footer');

    var sizeTop = nav.offset().top + nav.height() + offsetApp + topApp;
    var sizeWrap = $(window).height() - sizeTop - (bottom2.height() + 10);
    wrap.css('min-height', sizeWrap + "px");
}