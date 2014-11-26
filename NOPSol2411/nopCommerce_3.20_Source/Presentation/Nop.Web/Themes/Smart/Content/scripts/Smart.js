/// <reference path="..\..\..\..\Scripts\jquery-1.7.1.js" />

$(document).ready(function () {
    var responsiveMenuSettings = {
        doesBackgroundChange: false,
        doesPaddingChange: false,
        bgSelector: ".mega-menu-responsive > li > ul, .top-menu > li > .sublist",
        bgInitialColor: $(".menu-title span").css("background-color"),
        red: 40,
        green: 40,
        blue: 40,
        alpha: 0.05,
        paddingSelector: ".header-menu > ul > li",
        paddingValue: 0,
        themeBreakpoint: 1000
    }

    var responsiveAppSettings = {
        isEnabled: true,
        isSearchBoxDetachable: false,
        isHeaderLinksWrapperDetachable: false,
        doesDesktopHeaderMenuStick: true,
        doesScrollAfterFiltration: true,
        doesSublistHasIndent: true,
        displayGoToTop: true,
        hasStickyNav: true,
        selectors: {
            menuTitle: ".menu-title",
            headerMenu: ".header-menu",
            closeMenu: ".close-menu",
            movedElements: ".admin-header-links, .header, .responsive-nav-wrapper, .slider-wrapper, .master-wrapper-main, .footer",
            sublist: ".header-menu .sublist",
            overlayOffCanvas: ".overlayOffCanvas",
            withSubcategories: ".with-subcategories",
            filtersContainer: ".nopAjaxFilters7Spikes",
            filtersOpener: ".filters-button span",
            searchBox: ".search-box",
            searchBoxBefore: ".header-logo",
            navWrapper: ".responsive-nav-wrapper",
            navWrapperParent: ".responsive-nav-wrapper-parent",
            headerLinksWrapper: ".header-links-wrapper",
            shoppingCartLink: ".shopping-cart-link"
        }
    }

    initResponsiveTheme(responsiveMenuSettings, responsiveAppSettings);
});