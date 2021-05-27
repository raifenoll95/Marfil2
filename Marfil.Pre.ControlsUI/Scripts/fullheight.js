
$(document).ready(wrapResize);
$(window).resize(wrapResize);

function wrapResize() {
    var nav = $('#navbar');
    var navApp = $('#navbarApplication');
    
    var wrap = $('.full-height');
    
    var bottom2 = $('#footer');

    var sizeTop = nav.offset().top + nav.height() + navApp.offset().top + navApp.height();
    var sizeWrap = $(window).height() - sizeTop - (bottom2.height()) + 10;
    wrap.css('min-height', sizeWrap + "px");
}