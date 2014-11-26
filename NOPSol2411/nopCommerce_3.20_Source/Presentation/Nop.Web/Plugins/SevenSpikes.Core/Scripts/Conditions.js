$(document).ready(function () {

    DisplayExistingConditions();

    $("a.add-condition-group").click(function () {
        AddConditionGroup();
    });

    $(".k-grid-toolbar a.k-grid-delete").livequery("click", function () {
        var grid = $(this).closest(".condition-group-grid");
        if (grid != undefined) {
            var conditionGroupId = $(grid).attr("data-conditionGroupId");
            var gridId = $(grid).attr("id");

            if (conditionGroupId != undefined && gridId != undefined) {

                DeleteConditionGroup(conditionGroupId, gridId);
            }
        }
    });

    $("#conditionInfo #ConditionName, #conditionInfo #Active").change(function () {

        UpdateCondition();
    });

    $("#DefaultConditionValue").change(function () {
        UpdateDefaultConditionStatement();
    });

});

$("#editor").livequery(function () {
    AddViewModel();
});

$(window).unload(function () {

    var deleteUnusedConditionGroupsUrl = $.getHiddenValFromDom("#delete-unused-condition-groups-url");
    var conditionId = $.getHiddenValFromDom("#condition-id");
    var parameters = { "conditionId": conditionId };

    $.ajax({
        cache: false, async: false, type: "POST", data: $.toJSON(parameters),
        contentType: "application/json; charset=utf-8", url: deleteUnusedConditionGroupsUrl, success: function () {
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Deleting condition failed.");
        }
    });
});

function UpdateCondition() {

    var updateConditionUrl = $.getHiddenValFromDom("#update-condition-url");
    var conditionId = $.getHiddenValFromDom("#condition-id");
    var conditionInfo = $("#conditionInfo *").serialize();
    conditionInfo += "&" + "conditionId=" + conditionId;

    $.ajax({
        cache: false, async: false, type: "POST", data: conditionInfo,
        url: updateConditionUrl, success: function () {
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Updating condition failed.");
        }
    });
}

function UpdateDefaultConditionStatement() {
    var updateStatementUrl = $.getHiddenValFromDom("#update-default-condition-group-statement-url");
    var conditionId = $.getHiddenValFromDom("#condition-id");
    var updateValue = $('#DefaultConditionValue').find(":selected").val();

    $.ajax({
        cache: false, async: false, type: "POST", data: { conditionId: conditionId, defaultConditionStatementValue: updateValue },
        url: updateStatementUrl, success: function () {
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Updating condition default group statement failed.");
        }
    });
}

function DisplayExistingConditions() {

    var conditionGroupIds = $.parseJSON($.getHiddenValFromDom("#condition-groups"));
    if (conditionGroupIds != null) {
        for (var i = 0; i < conditionGroupIds.length; i++) {
            AddConditionGroup(conditionGroupIds[i]);
        }
    }
}

function AddConditionGroup(conditionGroupId) {

    if (conditionGroupId && conditionGroupId != 0) {
        AddConditionGroupGridHtml(conditionGroupId);
    } else {
        conditionGroupId = CreateConditionGroupAndAddConditionGroupGridHtml();
        if (conditionGroupId == "") {
            return;
        }
    }

    addKendoGridForConditionGroup(conditionGroupId);
}

function addKendoGridForConditionGroup(conditionGroupId) {
    var conditionGroupGridId = "condition-group-grid-" + conditionGroupId;

    var readConditionGroupUrl = $.getHiddenValFromDom("#read-condition-group-url");
    var updateConditionGroupUrl = $.getHiddenValFromDom("#update-condition-statement-url");
    var destroyConditioGroupUrl = $.getHiddenValFromDom("#destroy-condition-statement-url");
    var createConditionUrl = $.getHiddenValFromDom("#create-condition-statement-url");

    var popupValues = {};

    var conditionStatementId = 0;

    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: readConditionGroupUrl,
                dataType: "json",
                contentType: "application/json"
            },
            update: {
                url: updateConditionGroupUrl,
                type: "POST",
                dataType: "json",
                contentType: "application/json"
            },
            destroy: {
                url: destroyConditioGroupUrl,
                dataType: "json",
                contentType: "application/json"
            },
            create: {
                url: createConditionUrl,
                type: "POST",
                dataType: "json",
                contentType: "application/json"
            },
            parameterMap: function (options, operation) {
                if (operation === "update" || operation === "create") {
                    return kendo.stringify(popupValues);
                }
                if (operation === "read") {
                    return { conditionGroupId: kendo.stringify(conditionGroupId) };
                }
                if (operation === "destroy") {
                    return { conditionStatementId: kendo.stringify(conditionStatementId) };
                }
            }
        },
        batch: true,
        pageSize: 30,
        schema: {
            model: {
                id: "Id",
                fields: {
                    ConditionType: { type: "text" },
                    ConditionPropertyValue: { type: "number" },
                    ConditionPropertyText: { type: "text" },
                    OperatorTypeText: { type: "text" },
                    OperatorTypeValue: { type: "number" },
                    Text: { type: "text" },
                    Value: { type: "text" }
                }
            }
        }
    });

    $("#" + conditionGroupGridId).kendoGrid({
        dataSource: dataSource,
        sortable: true,
        editable: {
            mode: "popup",
            template: kendo.template($("#popup_editor").html())
        },
        edit: function (e) {
            var model = e.model;

            if (model.ConditionType != undefined && model.ConditionPropertyValue != undefined && model.OperatorTypeValue != undefined && model.Value != undefined) {
                $("#condition-type").attr("data-editValue", model.ConditionType);
                $("#condition-property").attr("data-editValue", model.ConditionPropertyValue);
                $("#condition-operator").attr("data-editValue", model.OperatorTypeValue);
                $("#condition-value").attr("data-editValue", model.Value);
                $("#condition-num-value").attr("data-editValue", model.Value);
                $("#condition-text-value").attr("data-editValue", model.Value);
            }
        },
        save: function (e) {
            popupValues.ConditionGroupId = conditionGroupId;
            popupValues.ConditionType = $("#condition-type").val();
            popupValues.ConditionPropertyValue = $("#condition-property").val();
            popupValues.OperatorTypeValue = $("#condition-operator").val();

            var valueDropdownDisplay = $("#condition-value").parents(".k-dropdown").css("display");
            var valueNumericTextboxDisplay = $("#condition-num-value").parents(".k-numerictextbox").css("display");
            var valueTextboxDisplay = $("#condition-text-value").css("display");

            if (!valueDropdownDisplay || valueDropdownDisplay != "none") {
                popupValues.Value = $("#condition-value").val();
            }
            else if (!valueNumericTextboxDisplay || valueNumericTextboxDisplay != "none") {
                popupValues.Value = $("#condition-num-value").val();
            }
            else if (!valueTextboxDisplay || valueTextboxDisplay != "none") {
                popupValues.Value = $("#condition-text-value").val();
            }

            // if this is true, then we are editing a row and not creating a new one
            if (e.model.ConditionType != undefined) {
                var data = dataSource.data();
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Id == e.model.Id) {
                        popupValues.Id = e.model.Id;
                        data[i].dirty = true;
                    }
                }
            }
        },
        remove: function (e) {
            if (e.model.Id != undefined && e.model.Id != "") {
                var data = dataSource.data();
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Id == e.model.Id) {
                        conditionStatementId = e.model.Id;
                        data[i].dirty = true;
                    }
                }
            }
        },
        toolbar: ["create", "destroy"],
        columns: [
            {
                field: "Id",
                title: "Id",
                width: 100,
                hidden: true
            },
            {
                field: "ConditionType",
                title: "Type",
                width: 100
            },
            {
                field: "ConditionPropertyText",
                title: "Property",
                width: 100
            },
            {
                field: "OperatorTypeText",
                title: "OperatorType",
                width: 100

            },
            {
                field: "Text",
                title: "Text",
                width: 250
            },
             {
                 field: "Value",
                 title: "Value",
                 width: 200,
                 hidden: true
             },
             { command: ["edit", "destroy"], title: "&nbsp;", width: "150px" }
        ]
    });
}

function DeleteConditionGroup(conditionGroupId, gridId) {
    var deleteConditionGroupUrl = $.getHiddenValFromDom("#delete-condition-group-url");

    var parameters = { "conditionGroupId": conditionGroupId };

    $.ajax({
        cache: false, type: "POST", data: $.toJSON(parameters),
        contentType: "application/json; charset=utf-8", url: deleteConditionGroupUrl, success: function () {

            $("#" + gridId).data("kendoGrid").destroy();
            $("#" + gridId).parent().remove();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Deleting condition group failed.");
        }
    });

    return;
}

function AddViewModel() {
    var viewModel = kendo.observable({
        typeSource: GetConditionTypes(),
        selectedType: null,
        selectedProperty: null,
        selectedOperator: null,
        selectedValue: null
    });

    viewModel.selectedType = SetSelectedConditionType(viewModel.typeSource);

    viewModel.selectedProperty = SetSelectedConditionProperty(viewModel.selectedType);

    viewModel.selectedOperator = SetSelectedOperatorType(viewModel.selectedProperty);

    viewModel.selectedValue = function () {

        var valueDropdownDisplay = $("#condition-value").parents(".k-dropdown").css("display");
        var valueNumericTextboxDisplay = $("#condition-num-value").parents(".k-numerictextbox").css("display");
        var valueTextboxDisplay = $("#condition-text-value").css("display");

        var value;

        if (!valueDropdownDisplay || valueDropdownDisplay != "none") {

            value = $("#condition-value").attr("data-editValue");
        }
        else if (!valueNumericTextboxDisplay || valueNumericTextboxDisplay != "none") {

            value = $("#condition-num-value").attr("data-editValue");
        }
        else if (!valueTextboxDisplay || valueTextboxDisplay != "none") {

            value = $("#condition-text-value").attr("data-editValue");
        }

        if (value != undefined && value != "") {

            return value;
        }

        return null;
    };

    kendo.bind($("#editor"), viewModel);
}

function SetSelectedConditionType(conditionTypes) {

    var conditionType = $("#condition-type").attr("data-editValue");

    if (conditionType != undefined && conditionType != "") {

        for (var i = 0; i < conditionTypes.length; i++) {

            if (conditionTypes[i].ConditionType == conditionType) {

                return conditionTypes[i];
            }
        }
    }

    return null;
}

function SetSelectedConditionProperty(conditionType) {

    var conditionProperty = $("#condition-property").attr("data-editValue");

    if (conditionProperty != undefined && conditionProperty != "") {

        for (var i = 0; i < conditionType.ConditionProperties.length; i++) {

            if (conditionType.ConditionProperties[i].ConditionPropertyValue == conditionProperty) {

                return conditionType.ConditionProperties[i];
            }
        }
    }

    return null;
}

function SetSelectedOperatorType(selectedConditionProperty) {

    var operatorTypeValue = $("#condition-operator").attr("data-editValue");

    if (operatorTypeValue != undefined && operatorTypeValue != "") {

        for (var i = 0; i < selectedConditionProperty.Operators.length; i++) {

            if (selectedConditionProperty.Operators[i].OperatorTypeValue == operatorTypeValue) {

                return selectedConditionProperty.Operators[i];
            }
        }
    }

    return null;
}

function GetConditionTypes() {
    var getConditionTypesUrl = $.getHiddenValFromDom("#get-condition-type-url");
    var getAvailableConditionTypes = $.getHiddenValFromDom("#available-condition-types");

    var conditionTypes;
    $.ajax({
        cache: false, async: false, type: "POST", contentType: "application/json; charset=utf-8", data: $.toJSON({ availableConditionTypesIds: getAvailableConditionTypes }),
        url: getConditionTypesUrl, success: function (data) {

            conditionTypes = data;

        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Retrieving condition types failed.");
        }
    });

    return conditionTypes;
}

function CreateConditionGroupAndAddConditionGroupGridHtml() {

    var createConditionGroupUrl = $.getHiddenValFromDom("#create-condition-group-url");
    var conditionId = $.getHiddenValFromDom("#condition-id");

    if (createConditionGroupUrl == "" || conditionId == "") {
        return 0;
    }

    var parameters = { "conditionId": conditionId };

    var conditionGroupId = "";

    $.ajax({
        cache: false, async: false, type: "POST", data: $.toJSON(parameters),
        contentType: "application/json; charset=utf-8", url: createConditionGroupUrl, success: function (data) {
            conditionGroupId = data;
            AddConditionGroupGridHtml(conditionGroupId);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Creating new condition group failed.");
        }
    });

    return conditionGroupId;
}

function AddConditionGroupGridHtml(conditionGroupId) {
    var conditionGroupGridClass = "condition-group-grid";
    var conditionGroupGridId = "condition-group-grid-" + conditionGroupId;

    var conditionGroupGridHtml = "<div><div class=\"" + conditionGroupGridClass + "\" id=\"" + conditionGroupGridId + "\" data-conditionGroupId=\"" + conditionGroupId + "\"></div><div class=\"group-dependancy-text\"><p>--OR--</p></div></div>";
    $("#condition-groups-wrapper").append(conditionGroupGridHtml);
}