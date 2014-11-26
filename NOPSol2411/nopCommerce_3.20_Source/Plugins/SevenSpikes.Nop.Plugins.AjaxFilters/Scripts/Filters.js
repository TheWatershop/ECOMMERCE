/// <reference path="..\..\..\Presentation\Nop.Web\Scripts\jquery-1.5.1.js" />
/// <reference path="..\..\..\Presentation\Nop.Web\Scripts\jquery-ui.js" />
/// <reference path="..\..\..\Presentation\Nop.Web\Plugins\SevenSpikes.Nop.Plugins.AjaxFilters\Scripts\jquery.query-2.1.7.js" />
/// <reference path="..\..\..\Presentation\Nop.Web\Plugins\SevenSpikes.Nop.Plugins.AjaxFilters\Scripts\jquery.json-2.2.min.js" />

$(document).ready(function () {

    if (typeof String.prototype.trim !== 'function') {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, '');
        }
    }

    var filtersManager = new FiltersManager();
    filtersManager.addProductPanelAjaxBusyToPage();

    $(".nopAjaxFilters7Spikes .block .title a.toggleControl").on("click", function () {
        if ($(this).hasClass("closed") == true) {
            $(this).removeClass("closed");
        }
        else {
            $(this).addClass("closed");
        }
        $(this).parent().siblings(".filtersGroupPanel").slideToggle("slow", function () {
            // remove the overflow:hidden which is left after the dropdown is shown. This is a know bug with jquery and is 
            // due to the box-sizing:border-box; See ticket here: http://bugs.jquery.com/ticket/12854
            $(this).css("overflow", "");
        });
    });

    $(".nopAjaxFilters7Spikes .block .title a.clearFilterOptions").each(function () {

        $(this).click({ "filtersManager": filtersManager }, filtersManager.clearFilterOptions);
        $(this).hide();
    });

    $(".nopAjaxFilters7Spikes .filtersDropDown a.allFilterDropDownOptions").each(function () {

        $(this).click({ "filtersManager": filtersManager }, filtersManager.selectAllFilterDropDownOption);
    });

    var clearAllButton = $(".nopAjaxFilters7Spikes .filtersTitlePanel a.clearFilterOptionsAll");
    clearAllButton.click({ "filtersManager": filtersManager }, filtersManager.clearAllFilterOptions);
    clearAllButton.hide();

    filtersManager.replaceSortAndViewOptionsDropDowns();
});

function FiltersManager() {

    this.categoryId = $(".nopAjaxFilters7Spikes").attr("data-categoryId");
    this.manufacturerId = $(".nopAjaxFilters7Spikes").attr("data-manufacturerId");
    this.vendorId = $(".nopAjaxFilters7Spikes").attr("data-vendorId");
    this.getFilteredProductsUrl = $(".nopAjaxFilters7Spikes").attr("data-getFilteredProductsUrl");
    this.filtersUIMode = $(".nopAjaxFilters7Spikes").attr("data-filtersUIMode");
    this.filtersUIModeEnum = { userCheckboxes: "usecheckboxes", useDropDowns: "usedropdowns" };
    this.selectedFilterGroupElement = undefined;
    this.selectedFilterOptionElement = undefined;

    this.requestProductsForSelectedFilters = RequestProductsForSelectedFilters;
    this.getSpecificationFiltersModel = GetSpecificationFiltersModel;
    this.getAttributeFiltersModel = GetAttributeFiltersModel;
    this.getManufacturerFiltersModel = GetManufacturerFiltersModel;
    this.getVendorFiltersModel = GetVendorFiltersModel;
    this.getPriceRangeFilterModel = GetPriceRangeFilterModel;
    this.refreshProducts = RefreshProducts;
    this.refreshProductsPager = RefreshProductsPager;
    this.replaceSortAndViewOptionsDropDowns = ReplaceSortAndViewOptionsDropDowns;
    this.setPagerLinks = SetPagerLinks;
    this.getProductsPanelSelector = GetProductsPanelSelector;
    this.getPagerPanelSelector = GetPagerPanelSelector;
    this.getSortOptionsDropDownSelector = GetSortOptionsDropDownSelector;
    this.getViewOptionsDropDownSelector = GetViewOptionsDropDownSelector;
    this.getProductsPageSizeDropDownSelector = GetProductsPageSizeDropDownSelector;
    this.sortProducts = SortProducts;
    this.changeViewMode = ChangeViewMode;
    this.changePageSize = ChangePageSize;
    this.clearFilterOptions = ClearFilterOptions;
    this.clearFilterOptionsForFiltersGroupElement = ClearFilterOptionsForFiltersGroupElement;
    this.selectAllFilterDropDownOption = SelectAllFilterDropDownOption;
    this.clearAllFilterOptions = ClearAllFilterOptions;
    this.refreshClearButtons = RefreshClearButtons;
    this.changeViewModeOrderByAndPageSize = ChangeViewModeOrderByAndPageSize;
    this.setPriceRangeValues = SetPriceRangeValues;
    this.refreshFilters = RefreshFilters;
    this.addProductPanelAjaxBusyToPage = function () {
        parentForAjaxBusyPanel = ".ajaxBusyPanelParent";

        var currentViewMode = $("#viewOptionsDropDown").attr("data-DropDownOptionsSelectedValue");

        if (currentViewMode == undefined) {
            currentViewMode = $(".nopAjaxFilters7Spikes").attr("data-defaultViewMode");
        }
        var productsPanelSelector = this.getProductsPanelSelector(currentViewMode);

        // There should be either a .product-grid panel or a .product-list panel
        var productGridListPanelSelector = $(".nopAjaxFilters7Spikes").attr("data-productsGridPanelSelector") + "," + $(".nopAjaxFilters7Spikes").attr("data-ProductsListPanelSelector");
        var productPanel = $(productGridListPanelSelector);

        if (productPanel.length > 1) {
            // Get the last element for the products grid if there are more than one.
            // This fixes an issue when the featured products are placed before the products panel and both have the same css selector
            // in which case we don't want to replace the featured products panel as it needs to always stay on top.
            $(productPanel[productPanel.length - 1]).addClass("ajaxBusyPanelParent");
        }
        else {
            $(productPanel).addClass("ajaxBusyPanelParent");
        }

        $(parentForAjaxBusyPanel).css("position", "relative"); // Make the parent relative because AjaxBusyPanel is absolute positioned
        $(parentForAjaxBusyPanel).append($("<div class=\"productPanelAjaxBusy\"></div><div class=\"clear\"></div>"));
        $(".productPanelAjaxBusy").hide();
    };

    this.setSpecificationFilterSelection = function (selectedGroupElement, selectedOptionElement) {
        this.selectedFilterGroupElement = selectedGroupElement;
        this.selectedFilterOptionElement = selectedOptionElement;
    };

    this.determineIfRequestIsValidForControlState = function (selectedOptionElement) {

        var validRequest = false;

        if (selectedOptionElement == undefined || selectedOptionElement.attr("class") == "filterItemUnselected" ||
            (selectedOptionElement.attr("class") == "filterItemSelected" && this.filtersUIMode == this.filtersUIModeEnum.userCheckboxes)) {
            validRequest = true;
        }

        return validRequest;
    };

    this.changeControlsState = function (selectedOptionElement) {

        if (selectedOptionElement == undefined) {
            return;
        }

        var optionClass = selectedOptionElement.attr("class");

        if (optionClass == "filterItemSelected") {
            selectedOptionElement.attr("class", "filterItemUnselected");
        }
        else if (optionClass == "filterItemUnselected") {
            selectedOptionElement.attr("class", "filterItemSelected");
        }

        if (this.selectedFilterGroupElement != undefined && this.filtersUIMode == this.filtersUIModeEnum.useDropDowns) {

            var selectedFilterOptionElement = this.selectedFilterOptionElement;

            this.selectedFilterGroupElement.find("a[data-option-id],[data-option-ids]").each(function (index, value) {

                if ($(value).attr("class") == "filterItemSelected" && value != selectedFilterOptionElement.get(0)) {
                    $(value).attr("class", "filterItemUnselected");
                }
            });
        }
    };

    this.showNoProductsModalDialog = function () {

        var noProductsWindow = $("#nopAjaxFiltersNoProductsDialog").data("kendoWindow");
        var noProductsWindowTitle = $("#nopAjaxFiltersNoProductsDialog").attr("title");
        if (!noProductsWindowTitle) {
            noProductsWindowTitle = "NO RESULTS FOUND";
        }

        if (!noProductsWindow) {
            noProductsWindow = $("#nopAjaxFiltersNoProductsDialog").kendoWindow({
                draggable: false,
                resizable: false,
                width: "300px",
                height: "100px",
                title: noProductsWindowTitle,
                modal: true,
                actions: ["Close"],
                animation: false,
                visible: false
            }).data("kendoWindow");

            noProductsWindow.wrapper.addClass("ajaxFilters");
        }

        noProductsWindow.center();
        noProductsWindow.open();
    };

    this.showProductPanelAjaxBusy = function () {
        $(".productPanelAjaxBusy").show();
    };

    this.hideProductPanelAjaxBusy = function () {
        $(".productPanelAjaxBusy").hide();
    };


}

FiltersManager.prototype.getFilterItemStateBasedOnItemClass = function (itemClass, currentState) {
    var filterItemState = currentState;

    switch (itemClass) {
        case "filterItemUnselected":
            filterItemState = "Unchecked";
            break;
        case "filterItemSelected":
            filterItemState = "Checked";
            break;
        case "filterItemSelectedDisabled":
            filterItemState = "CheckedDisabled";
            break;
        case "filterItemDisabled":
            filterItemState = "Disabled";
            break;
    }

    return filterItemState;
};

FiltersManager.prototype.getFilterItemClassBasedOnState = function (itemState, currentClass) {
    var filterItemClass = currentClass;

    switch (itemState) {
        case 0:
            filterItemClass = "filterItemUnselected";
            break;
        case 1:
            filterItemClass = "filterItemSelected";
            break;
        case 2:
            filterItemClass = "filterItemSelectedDisabled";
            break;
        case 3:
            filterItemClass = "filterItemDisabled";
            break;
    }

    return filterItemClass;
};

FiltersManager.waitForAjaxRequest = false;

function RequestProductsForSelectedFilters(selectedGroupElement, selectedOptionElement, pageNumber, hashQuery, noHashQueryInitialized, shouldNotStartFromFirstPage) {

    if (FiltersManager.waitForAjaxRequest) {
        return;
    }

    // This logic checks if the URL hash query has no filtering applied in it.
    // For example on refresh or if you go to product page and then press BACK button
    // we do not want AJAX request, if no filters are applied.
    if (hashQuery == "#/noFilterApplied") {
        return;
    }

    if (!this.determineIfRequestIsValidForControlState(selectedOptionElement)) {
        return;
    }

    if (hashQuery == undefined) {
        hashQuery = "";
    } else {
        shouldNotStartFromFirstPage = true;
    }

    this.setSpecificationFilterSelection(selectedGroupElement, selectedOptionElement);
    this.changeControlsState(selectedOptionElement);

    var specificationFiltersModel = this.getSpecificationFiltersModel();
    var attributeFiltersModel = this.getAttributeFiltersModel();
    var manufacturerFiltersModel = this.getManufacturerFiltersModel();
    var vendorFiltersModel = this.getVendorFiltersModel();
    var priceRangeFilterModel = this.getPriceRangeFilterModel();

    // If the pageNumber parameter in the getFilteredProductsParameters json object is undefined then
    // the jsong object is considered invalid and older browsers like IE 7 will throw an error.
    if (pageNumber == undefined) {
        pageNumber = null;
    }

    var sortOption = $("#sortOptionsDropDown").attr("data-DropDownOptionsSelectedValue");

    if (sortOption == undefined) {
        sortOption = null;
    }

    var viewMode = $("#viewOptionsDropDown").attr("data-DropDownOptionsSelectedValue");

    if (viewMode == undefined) {
        viewMode = null;
    }

    var productsPageSize = $("#productsPageSizeDropDown").attr("data-DropDownOptionsSelectedValue");

    if (productsPageSize == undefined) {
        productsPageSize = null;
    }

    var queryPageNumber = $.getUrlVar('pagenumber');

    // The page number should not be set when the noHashQuery is initialized for the second time.
    if (queryPageNumber != undefined && pageNumber == null && (noHashQueryInitialized == undefined || noHashQueryInitialized == false)) {
        pageNumber = queryPageNumber;
    }

    if (shouldNotStartFromFirstPage == undefined) {
        shouldNotStartFromFirstPage = false;
    }

    var getFilteredProductsParameters = { "categoryId": this.categoryId, "manufacturerId": this.manufacturerId, "vendorId": this.vendorId, "priceRangeFilterModel7Spikes": priceRangeFilterModel, "specificationFiltersModel7Spikes": specificationFiltersModel, "attributeFiltersModel7Spikes": attributeFiltersModel, "manufacturerFiltersModel7Spikes": manufacturerFiltersModel, "vendorFiltersModel7Spikes": vendorFiltersModel, "pageNumber": pageNumber, "orderby": sortOption, "viewmode": viewMode, "pagesize": productsPageSize, "queryString": hashQuery, "shouldNotStartFromFirstPage": shouldNotStartFromFirstPage };

    this.showProductPanelAjaxBusy();

    var filtersManager = this;
    var productsRequestUrl = this.getFilteredProductsUrl;

    FiltersManager.waitForAjaxRequest = true;

    $.ajax({
        cache: false, type: "POST", data: $.toJSON(getFilteredProductsParameters),
        contentType: "application/json; charset=utf-8", url: productsRequestUrl, success: function (data) {

            if (hashQuery != "") {
                // We need to make sure the Sort and View options are updated with the returned data before we call the 
                // refreshProducts function as otherwise it won't actually refresh the products and will throw an exception
                // This way we prevent this situation: Dropdown is Grid but the filtered products are returned in List and the product panel selector will fail
                filtersManager.changeViewModeOrderByAndPageSize(data);

                filtersManager.setPriceRangeValues(data);
            }

            var hasProducts = filtersManager.refreshProducts(data);

            if (!hasProducts) {

                filtersManager.changeControlsState(filtersManager.selectedFilterOptionElement);
                FiltersManager.waitForAjaxRequest = false;
                return;
            }

            filtersManager.refreshFilters(data);

            filtersManager.refreshProductsPager(data);

            filtersManager.refreshClearButtons();

            // Set the hash tag
            // The hash tag should not be set if the function is called as "NO HASH QUERY"
            if (hashQuery != "NO HASH QUERY") {

                var hashQueryStringFromModel = $(data).filter("#urlHashQuery").val().toString();
                if (hashQueryStringFromModel != '') {
                    $.address.value(hashQueryStringFromModel);
                }
                else {
                    $.address.value("noFilterApplied");
                }
            }

            filtersManager.hideProductPanelAjaxBusy();

            FiltersManager.waitForAjaxRequest = false;

            var scrollToElement = $(".nopAjaxFilters7Spikes").attr("data-scrollToElement");

            if (scrollToElement != undefined && scrollToElement.toLowerCase() == "true") {
                var elementToScrollToAfterFiltration = $(".nopAjaxFilters7Spikes").attr("data-scrollToElementSelector");
                var elementList = elementToScrollToAfterFiltration.split(",");
                var scrollToThisElement = ".product-selectors";

                if (elementList != undefined && elementList.length > 0) {
                    for (var c = 0; c < elementList.length; c++) {
                        if ($(elementList[c].trim()).length > 0) {
                            scrollToThisElement = elementList[c].trim();
                            break;
                        }
                    }
                    if ($(scrollToThisElement).length > 0) {
                        $('html, body').animate({ scrollTop: $(scrollToThisElement).offset().top }, "slow");
                    }
                }
            }

            if ($(".productPanelAjaxBusy").length < 1) {
                filtersManager.addProductPanelAjaxBusyToPage();
            }

            $.event.trigger({ type: "nopAjaxFiltersFiltrationCompleteEvent" });

        },
        error: function (jqXHR, textStatus, errorThrown) {
            filtersManager.hideProductPanelAjaxBusy();
            alert("Loading the page failed.");

            FiltersManager.waitForAjaxRequest = false;
        }
    });
}

function RefreshFilters(data) {
    var filtersManager = this;
    var specificationFilterModel7SpikesJson = $(data).filter("#specificationFilterModel7SpikesJson").val().toString();
    var specificationFilterModel7Spikes = $.parseJSON(specificationFilterModel7SpikesJson);

    if (filtersManager.refreshSpecificationFilters != undefined) {
        filtersManager.refreshSpecificationFilters(specificationFilterModel7Spikes, filtersManager.filtersUIMode);
    }

    var attributeFilterModel7SpikesJson = $(data).filter("#attributeFilterModel7SpikesJson").val().toString();
    var attributeFilterModel7Spikes = $.parseJSON(attributeFilterModel7SpikesJson);

    if (filtersManager.refreshAttributeFilters != undefined) {
        filtersManager.refreshAttributeFilters(attributeFilterModel7Spikes, filtersManager.filtersUIMode);
    }

    var manufacturerFiltersModel7SpikesJson = $(data).filter("#manufacturerFilterModel7SpikesJson").val().toString();
    var manufacturerFiltersModel7Spikes = $.parseJSON(manufacturerFiltersModel7SpikesJson);

    if (filtersManager.refreshManufacturerFilters != undefined) {
        filtersManager.refreshManufacturerFilters(manufacturerFiltersModel7Spikes, filtersManager.filtersUIMode);
    }

    var vendorFiltersModel7SpikesJson = $(data).filter("#vendorFilterModel7SpikesJson").val().toString();
    var vendorFiltersModel7Spikes = $.parseJSON(vendorFiltersModel7SpikesJson);

    if (filtersManager.refreshVendorFilters != undefined) {
        filtersManager.refreshVendorFilters(vendorFiltersModel7Spikes, filtersManager.filtersUIMode);
    }
}

function SetPriceRangeValues(data) {
    var filtersManager = this;
    var priceRangeFromJson = $(data).filter("#priceRangeFromJson").val().toString();
    var priceRangeFrom = $.parseJSON(priceRangeFromJson);

    var priceRangeToJson = $(data).filter("#priceRangeToJson").val().toString();
    var priceRangeTo = $.parseJSON(priceRangeToJson);

    if (filtersManager.SetSliderValues != undefined)
        filtersManager.SetSliderValues(priceRangeFrom, priceRangeTo);
}

function RequestProductsForPagerLink(eventObject) {

    var currentElement = $(eventObject.currentTarget);

    var pageNumber = currentElement.attr("data-pageNumber");

    if (eventObject.data != null && eventObject.data.filtersManager != undefined) {

        var filtersManager = eventObject.data.filtersManager;

        filtersManager.requestProductsForSelectedFilters(undefined, undefined, pageNumber, undefined, undefined, true);
    }
}

function GetSpecificationFiltersModel() {

    var specificationFiltersModel = null;

    if (this.buildSpecificationFiltersModel != undefined) {

        specificationFiltersModel = this.buildSpecificationFiltersModel(this.categoryId, this.manufacturerId, this.vendorId, this.selectedFilterGroupElement);
    }

    return specificationFiltersModel;
}

function GetAttributeFiltersModel() {

    var attributeFiltersModel = null;

    if (this.buildAttributeFiltersModel != undefined) {

        attributeFiltersModel = this.buildAttributeFiltersModel(this.categoryId, this.manufacturerId, this.vendorId, this.selectedFilterGroupElement);
    }

    return attributeFiltersModel;
}

function GetManufacturerFiltersModel() {

    var manufacturerFiltersModel = null;

    if (this.buildManufacturerFiltersModel != undefined) {

        manufacturerFiltersModel = this.buildManufacturerFiltersModel(this.categoryId, this.selectedFilterGroupElement);
    }

    return manufacturerFiltersModel;
}

function GetVendorFiltersModel() {

    var vendorFiltersModel = null;

    if (this.buildVendorFiltersModel != undefined) {
        vendorFiltersModel = this.buildVendorFiltersModel(this.categoryId, this.selectedFilterGroupElement);
    }

    return vendorFiltersModel;
}

function GetPriceRangeFilterModel() {

    var priceRangeFilterModel = null;

    if (this.buildPriceRangeFilterModel != undefined) {

        priceRangeFilterModel = this.buildPriceRangeFilterModel(this.categoryId, this.manufacturerId, this.vendorId, this.selectedFilterGroupElement);
    }

    return priceRangeFilterModel;
}

function RefreshProducts(productsData) {

    var currentViewMode = $("#viewOptionsDropDown").attr("data-DropDownOptionsSelectedValue");

    if (currentViewMode == undefined) {
        currentViewMode = $(".nopAjaxFilters7Spikes").attr("data-defaultViewMode");
    }

    var productsPanelSelector = this.getProductsPanelSelector(currentViewMode);

    // There should be either a .product-grid panel or a .product-list panel
    var productGridListPanelSelector = $(".nopAjaxFilters7Spikes").attr("data-productsGridPanelSelector") + "," + $(".nopAjaxFilters7Spikes").attr("data-ProductsListPanelSelector");
    var productPanel = $(productGridListPanelSelector);

    var hasProducts;

    if ($(productsData).filter(productsPanelSelector).html() != null) {

        if (productPanel.length > 1) {
            // Get the last element for the products grid if there are more than one.
            // This fixes an issue when the featured products are placed before the products panel and both have the same css selector
            // in which case we don't want to replace the featured products panel as it needs to always stay on top.
            $(productPanel[productPanel.length - 1]).replaceWith($(productsData).filter(productsPanelSelector));
        }
        else {
            productPanel.replaceWith($(productsData).filter(productsPanelSelector));
        }

        hasProducts = true;
    }
    else {
        this.hideProductPanelAjaxBusy();
        if ($("#nopAjaxFiltersNoProductsDialog").length > 0) {
            this.showNoProductsModalDialog();
        }
        else {
            productPanel.prepend($(productsData).filter("#nopAjaxFiltersNoProductsDialog"));
            this.showNoProductsModalDialog();
        }
        hasProducts = false;
    }

    return hasProducts;
}

function GetProductsPanelSelector(currentViewMode) {
    var productPanelSelector;
    if (currentViewMode == "list") {
        var productsListPanelSelector = $(".nopAjaxFilters7Spikes").attr("data-ProductsListPanelSelector");

        if (productsListPanelSelector != undefined) {
            productPanelSelector = productsListPanelSelector;
        }
        else {
            productPanelSelector = ".product-list";
        }
    }
    else {
        var productsGridPanelSelector = $(".nopAjaxFilters7Spikes").attr("data-productsGridPanelSelector");

        if (productsGridPanelSelector != undefined) {
            productPanelSelector = productsGridPanelSelector;
        }
        else {
            productPanelSelector = ".product-grid";
        }
    }

    return productPanelSelector;
}

function GetPagerPanelSelector() {

    var pagerPanelSelector = ".pager";

    if (pagerPanelSelector != undefined) {
        pagerPanelSelector = $(".nopAjaxFilters7Spikes").attr("data-pagerPanelSelector");
    }

    return pagerPanelSelector;
}

function RefreshProductsPager(data) {

    var pagerPanelSelector = this.getPagerPanelSelector();
    var pagerPanel = $(pagerPanelSelector);

    pagerPanel.html($(data).filter(pagerPanelSelector).html());

    this.setPagerLinks(pagerPanel);
}

function SetPagerLinks(pagerPanel) {

    var filtersMananger = this;

    pagerPanel.find("a").each(function () {

        var pagerLinkUrl = $(this).attr("href");

        var pageNumber = "";

        if (pagerLinkUrl != undefined) {

            var pageNumberRegex = /pagenumber=(\d+)/;
            pagerLinkUrl = pagerLinkUrl.toLowerCase();
            var match = pageNumberRegex.exec(pagerLinkUrl);
            if (match != null && match.length > 1) {
                pageNumber = match[1];
            }
        }

        $(this).removeAttr("href");
        $(this).attr("data-pageNumber", pageNumber);
        $(this).click({ "filtersManager": filtersMananger }, RequestProductsForPagerLink);
    });
}

function GetSortOptionsDropDownSelector() {

    var sortOptionsDropDownSelector = $(".nopAjaxFilters7Spikes").attr("data-sortOptionsDropDownSelector");

    if (sortOptionsDropDownSelector == "") {
        sortOptionsDropDownSelector = "#products-orderby";
    }

    return sortOptionsDropDownSelector;
}

function GetViewOptionsDropDownSelector() {

    var sortOptionsDropDownSelector = $(".nopAjaxFilters7Spikes").attr("data-viewOptionsDropDownSelector");

    if (sortOptionsDropDownSelector == "") {
        sortOptionsDropDownSelector = "#products-viewmode";
    }

    return sortOptionsDropDownSelector;
}

function GetProductsPageSizeDropDownSelector() {

    var productsPageSizeDropDownSelector = $(".nopAjaxFilters7Spikes").attr("data-productsPageSizeDropDownSelector");

    if (productsPageSizeDropDownSelector == "") {
        productsPageSizeDropDownSelector = "#products-pagesize";
    }

    return productsPageSizeDropDownSelector;
}

//This function is used to set the selected view mode, page size and order by when there are predefined filters and
// we want to reload them with the specified values i.e when we have a #hash tag in the url
function ChangeViewModeOrderByAndPageSize(data) {
    var currentPageSizeJson = $(data).filter("#currentPageSizeJson").val().toString();
    var selectedProductPageSize = $.parseJSON(currentPageSizeJson);

    var currentOrderByJson = $(data).filter("#currentOrderByJson").val().toString();
    var selectedOrderBy = $.parseJSON(currentOrderByJson);

    var currentViewModeJson = $(data).filter("#currentViewModeJson").val().toString();
    var selectedViewMode = $.parseJSON(currentViewModeJson);

    // If there are specified values for the dropdowns this means that the dropdowns have already been created, so we 
    // need to access them (the dropdown elements) via the newly set ids. This is for the productsPageSizeDropDown, viewOptionsDropDown and sortOptionsDropDown.
    var sortOptionsDropDown = $("#sortOptionsDropDown");
    var viewOptionsDropDown = $("#viewOptionsDropDown");
    var productsPageSizeDropDown = $("#productsPageSizeDropDown");

    if (sortOptionsDropDown.length == 0 && viewOptionsDropDown.length == 0 && productsPageSizeDropDown.length == 0) {

        return;
    }

    var sortOptions = new Array();
    var viewOptions = new Array();
    var pageSizes = new Array();

    var availableSortOptionsSelector = "#availableSortOptionsJson";
    var selectedSortIndex = PopulateOptions(sortOptionsDropDown, sortOptions, availableSortOptionsSelector, selectedOrderBy);

    var availableViewModeSelector = "#availableViewModesJson";
    var selectedViewMdodeIndex = PopulateOptions(viewOptionsDropDown, viewOptions, availableViewModeSelector, selectedViewMode);

    var availablePageSizeSelector = "#availablePageSizesJson";
    var selectedPageSizeIndex = PopulateOptions(productsPageSizeDropDown, pageSizes, availablePageSizeSelector, selectedProductPageSize);

    if (selectedProductPageSize != undefined) {
        SetSelectedElementByDropdownSelector(productsPageSizeDropDown, selectedPageSizeIndex, pageSizes);
    }

    if (selectedOrderBy != undefined) {
        SetSelectedElementByDropdownSelector(sortOptionsDropDown, selectedSortIndex, sortOptions);
    }

    if (selectedViewMode != undefined) {
        SetSelectedElementByDropdownSelector(viewOptionsDropDown, selectedViewMdodeIndex, viewOptions);
    }
}

function SetSelectedElementByDropdownSelector(dropDownSelector, elementIndex, dropdownOptions) {
    var hndlElement = dropDownSelector.get(0);
    var selectedElement = $(dropDownSelector.find("li")[elementIndex]);

    if (selectedElement != undefined && hndlElement != undefined) {
        hndlElement.selectedLiElement = selectedElement;
        hndlElement.par.text($(selectedElement).text());

        dropDownSelector.attr("data-dropDownOptionsSelectedValue", dropdownOptions[elementIndex].dropDownOptionValue);
    }
    else {
        return;
    }
}

function ReplaceSortAndViewOptionsDropDowns() {

    var sortOptionsDropDownSelector = this.getSortOptionsDropDownSelector();
    var sortOptionsDropDown = $(sortOptionsDropDownSelector);

    var viewOptionsDropDownSelector = this.getViewOptionsDropDownSelector();
    var viewOptionsDropDown = $(viewOptionsDropDownSelector);

    var productsPageSizeDropDownSelector = this.getProductsPageSizeDropDownSelector();
    var productsPageSizeDropDown = $(productsPageSizeDropDownSelector);


    if (sortOptionsDropDown.length == 0 && viewOptionsDropDown.length == 0 && productsPageSizeDropDown.length == 0) {

        return;
    }

    var sortOptions = new Array();
    var viewOptions = new Array();
    var pageSizes = new Array();

    var availableSortOptionsSelector = "#availableSortOptionsJson";
    var selectedSortIndex = PopulateOptions(sortOptionsDropDown, sortOptions, availableSortOptionsSelector);

    var availableViewModeSelector = "#availableViewModesJson";
    var selectedViewMdodeIndex = PopulateOptions(viewOptionsDropDown, viewOptions, availableViewModeSelector);

    var availablePageSizeSelector = "#availablePageSizesJson";
    var selectedPageSizeIndex = PopulateOptions(productsPageSizeDropDown, pageSizes, availablePageSizeSelector);

    var jDropDownTemplateUrl = nop_store_directory_root + "Plugins/SevenSpikes.Nop.Plugins.AjaxFilters/JQueryTemplates/JDropDown.htm";

    $.ajax({
        url: jDropDownTemplateUrl, async: false, success: function (template) {

            $.template("jDropDownTemplate", template);

            if (sortOptionsDropDown.length > 0) {

                var sortOptionsInfo = { jDropDownId: "sortOptionsDropDown", jDropDownCss: "sortOptionsDropDown", dropDownOptions: sortOptions };
                $.tmpl("jDropDownTemplate", sortOptionsInfo, { selectedOptionIndex: selectedSortIndex, dropDownOptionClickCallback: "new FiltersManager().sortProducts" }).insertAfter(sortOptionsDropDown);
                $("#sortOptionsDropDown").attr("data-dropDownOptionsSelectedValue", sortOptions[selectedSortIndex].dropDownOptionValue);
                sortOptionsDropDown.remove();
            }

            if (viewOptionsDropDown.length > 0) {

                var viewOptionsInfo = { jDropDownId: "viewOptionsDropDown", jDropDownCss: "viewOptionsDropDown", dropDownOptions: viewOptions };
                $.tmpl("jDropDownTemplate", viewOptionsInfo, { selectedOptionIndex: selectedViewMdodeIndex, dropDownOptionClickCallback: "new FiltersManager().changeViewMode" }).insertAfter(viewOptionsDropDown);
                $("#viewOptionsDropDown").attr("data-dropDownOptionsSelectedValue", viewOptions[selectedOptionIndex].dropDownOptionValue);
                viewOptionsDropDown.remove();
            }

            if (productsPageSizeDropDown.length > 0) {

                var pageSizesInfo = { jDropDownId: "productsPageSizeDropDown", jDropDownCss: "productsPageSizeDropDown", dropDownOptions: pageSizes };
                $.tmpl("jDropDownTemplate", pageSizesInfo, { selectedOptionIndex: selectedPageSizeIndex, dropDownOptionClickCallback: "new FiltersManager().changePageSize" }).insertAfter(productsPageSizeDropDown);
                $("#productsPageSizeDropDown").attr("data-dropDownOptionsSelectedValue", pageSizes[selectedPageSizeIndex].dropDownOptionValue);
                productsPageSizeDropDown.remove();
            }

        }, error: function (jqXHR, textStatus, errorThrown) {
            alert("Retrieving " + jDropDownTemplateUrl + " failed.");
        }
    });
}


function PopulateOptions(optionsDropDown, options, selector, selectedOption) {
    var selectedIndex = 0;
    if (optionsDropDown.length > 0) {

        var availableOptionsJson = $(selector).val().toString();
        var availableOptions = $.parseJSON(availableOptionsJson);

        $.each(availableOptions, function (index, value) {

            options.push({ dropDownOptionName: value.Text, dropDownOptionValue: value.Value });

            if (selectedOption) {
                if (value.Value == selectedOption) {
                    selectedIndex = index;
                }
            }
            else {
                if (selectedIndex == 0 && value.Selected) {
                    selectedIndex = index;
                }
            }
        });
    }

    return selectedIndex;
}

function SortProducts(eventObject) {

    var currentElement = $(eventObject.currentTarget);

    var sortOptionValue = currentElement.attr("data-DropDownOptionValue");

    $("#sortOptionsDropDown").attr("data-DropDownOptionsSelectedValue", sortOptionValue);

    var filtersManager = new FiltersManager();

    filtersManager.requestProductsForSelectedFilters();
}

function ChangeViewMode(eventObject) {

    var currentElement = $(eventObject.currentTarget);

    var viewModeValue = currentElement.attr("data-DropDownOptionValue");

    $("#viewOptionsDropDown").attr("data-DropDownOptionsSelectedValue", viewModeValue);

    var filtersManager = new FiltersManager();

    filtersManager.requestProductsForSelectedFilters(undefined, undefined, undefined, undefined, undefined, true);
}

function ChangePageSize(eventObject) {

    var currentElement = $(eventObject.currentTarget);

    var productsPageSize = currentElement.attr("data-DropDownOptionValue");

    $("#productsPageSizeDropDown").attr("data-DropDownOptionsSelectedValue", productsPageSize);

    var filtersManager = new FiltersManager();

    filtersManager.requestProductsForSelectedFilters();
}

function SelectAllFilterDropDownOption(eventObject) {

    var currentElement = $(eventObject.currentTarget);

    var filtersGroupPanel = currentElement.parents(".filtersGroupPanel");
    var filtersManager = eventObject.data.filtersManager;
    filtersManager.clearFilterOptionsForFiltersGroupElement(filtersGroupPanel);
}

function ClearFilterOptions(eventObject) {

    var currentElement = $(eventObject.currentTarget);
    var filtersGroupPanel = currentElement.parent().siblings(".filtersGroupPanel");
    var filtersManager = eventObject.data.filtersManager;
    filtersManager.clearFilterOptionsForFiltersGroupElement(filtersGroupPanel);
}

function ClearFilterOptionsForFiltersGroupElement(filtersGroupElement) {

    $(filtersGroupElement).find("a[data-option-id], a[data-optionsGroupId]").each(function (filterOptionIndex, filterOptionElement) {

        if ($(filterOptionElement).attr("class") == "filterItemSelected") {
            $(filterOptionElement).attr("class", "filterItemUnselected");
        }
        else if ($(filterOptionElement).attr("class") == "filterItemSelectedDisabled") {
            $(filterOptionElement).attr("class", "filterItemDisabled");
        }
    });

    if (this.filtersUIMode == this.filtersUIModeEnum.useDropDowns) {

        $(filtersGroupElement).find(".filtersDropDown").jDropDown({ method: "reset" });
    }

    var currentGroupElement = filtersGroupElement;

    this.requestProductsForSelectedFilters(currentGroupElement, undefined);

}

function ClearAllFilterOptions(eventObject) {

    $(".nopAjaxFilters7Spikes .filtersGroupPanel").each(function (index, value) {

        $(value).find("a[data-option-id], a[data-optionsGroupId]").each(function (filterOptionIndex, filterOptionElement) {

            $(filterOptionElement).attr("class", "filterItemUnselected");
        });
    });

    var filtersManager = eventObject.data.filtersManager;

    if (filtersManager.filtersUIMode == filtersManager.filtersUIModeEnum.useDropDowns) {

        $(".nopAjaxFilters7Spikes .filtersGroupPanel .filtersDropDown").jDropDown({ method: "reset" });
    }

    if (filtersManager.clearPriceRangeFilterControl != undefined) {
        filtersManager.clearPriceRangeFilterControl();
    }

    filtersManager.requestProductsForSelectedFilters();
}

function RefreshClearButtons() {

    var clearAllButtonEnabled = false;

    $(".nopAjaxFilters7Spikes .filtersGroupPanel").each(function (index, value) {

        var clearButtonEnabled = false;

        $(value).find("a[data-option-id], a[data-optionsGroupId]").each(function (filterOptionIndex, filterOptionElement) {

            if ($(filterOptionElement).attr("class") == "filterItemSelected") {
                clearButtonEnabled = true;
                return false;
            }
        });

        var clearButton = $(value).parent().find("a.clearFilterOptions");

        if (clearButtonEnabled) {
            clearButton.show();
            clearAllButtonEnabled = true;
        }
        else {
            clearButton.hide();
        }
    });

    var clearAllButton = $(".nopAjaxFilters7Spikes .filtersTitlePanel a.clearFilterOptionsAll");

    if (clearAllButtonEnabled) {
        clearAllButton.show();
    }
    else {

        var priceRangeHasBeenSelected = false;

        if (this.determineIfPriceRangeHasBeenSelected != undefined) {
            priceRangeHasBeenSelected = this.determineIfPriceRangeHasBeenSelected();
        }

        if (priceRangeHasBeenSelected) {
            clearAllButton.show();
        }
        else {
            clearAllButton.hide();
        }
    }
}