var surveyName = "";
function setSurveyName(name) {
    var $titleTitle = jQuery("#sjs_survey_creator_title_show");
    $titleTitle.find("span:first-child").text(name);
}
function startEdit() {
    var $titleSurveyCreator = jQuery("#sjs_survey_creator_title_edit");
    var $titleTitle = jQuery("#sjs_survey_creator_title_show");
    $titleTitle.hide();
    $titleSurveyCreator.show();
    $titleSurveyCreator.find("input")[0].value = surveyName;
    $titleSurveyCreator.find("input").focus();
}
function cancelEdit() {
    var $titleSurveyCreator = jQuery("#sjs_survey_creator_title_edit");
    var $titleTitle = jQuery("#sjs_survey_creator_title_show");
    $titleSurveyCreator.hide();
    $titleTitle.show();
}
function postEdit() {
    cancelEdit();
    var oldName = surveyName;
    var $titleSurveyCreator = jQuery("#sjs_survey_creator_title_edit");
    surveyName = $titleSurveyCreator.find("input")[0].value;
    setSurveyName(surveyName);
    jQuery
        .get("/changeName?id=" + surveyId + "&name=" + surveyName, function (data) {
            //surveyId = surveyName;
            window.location = "NewFormCreator?name=" + surveyName + "&id=" + surveyId;
        })
        .fail(function (error) {
            surveyName = oldName;
            setSurveyName(surveyName);
            alert(JSON.stringify(error));
        });
}

function getParams() {
    var url = window.location.href
        .slice(window.location.href.indexOf("?") + 1)
        .split("&");
    var result = {};
    url.forEach(function (item) {
        var param = item.split("=");
        result[param[0]] = param[1];
    });
    return result;
}

Survey.dxSurveyService.serviceUrl = "";
var accessKey = "";
var surveyCreator = new SurveyCreator.SurveyCreator("survey-creator");
//dummy start
surveyCreator
    .toolbarItems
    .push({
        id: "saveJsonTOSql",
        visible: true,
        title: "Save For Approve",
        action: function () {
            // var testSurveyModel = new Survey.Model(creator.getSurveyJSON());
            // testSurveyModel.render("surveyContainerInPopup");
            // modal.open();
            $.post("/Admin/SaveForApprove", { Id: decodeURI(getParams()["id"]) }, function (data, status) {
                if (data == 2)
                    alert("Data Saved Successfully. ");
            });
        }
    });
//dummy end
var surveyId = decodeURI(getParams()["id"]);
surveyCreator.loadSurvey(surveyId);
surveyCreator.saveSurveyFunc = function (saveNo, callback) {
    var xhr = new XMLHttpRequest();
    xhr.open(
        "POST",
        Survey.dxSurveyService.serviceUrl + "/changeJson?accessKey=" + accessKey
    );
    xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhr.onload = function () {
        var result = xhr.response ? JSON.parse(xhr.response) : null;
        if (xhr.status === 200) {
            callback(saveNo, true);
        }
    };
    xhr.send(
        JSON.stringify({
            Id: surveyId,
            Json: surveyCreator.text,
            Text: surveyCreator.text
        })
    );
};
//add file type into matrix columns (run-time)
Survey.matrixDropdownColumnTypes.file = { properties: ["showPreview", "imageHeight", "imageWidth"] };
//add file type into matrix columns (design-time/editor)
SurveyCreator.SurveyQuestionEditorDefinition.definition["matrixdropdowncolumn@file"] = {
    properties: ["showPreview", "imageHeight", "imageWidth"],
    tabs: [{ name: "visibleIf", index: 12 }, { name: "enableIf", index: 20 }]
};
//Add a tag property to all questions
Survey
    .Serializer
    .addProperty("question", "tag");
// Change the order of name and title properties, remove the startWithNewLine property and add a tag property
SurveyCreator
    .SurveyQuestionEditorDefinition
    .definition["question"]
    .properties = [
        "title",
        "name", {
            name: "tag",
            title: "Tag"
        }, {
            name: "visible",
            category: "checks"
        }, {
            name: "isRequired",
            category: "checks"
        }

    ];
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "QueryString",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 3
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("QueryString", ShortTextEditor);
//----------------------------------
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "CascadeControlName",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 11
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("select");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("CascadeControlName", ShortTextEditor);
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "ApiUrl",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 13
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("ApiUrl", ShortTextEditor);
////----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "DepandantControlName",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 11
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("select");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("DepandantControlName", ShortTextEditor);
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "ValueFiled",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 16
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("ValueFiled", ShortTextEditor);
////----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "DisplayFiled",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 17
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("DisplayFiled", ShortTextEditor);
////----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "InternalApi",
        type: "boolean",
        isRequired: true,
        category: "general",
        visibleIndex: 12
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "boolean";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("InternalApi", ShortTextEditor);
////----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "ApiParameter",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 14
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("ApiParameter", ShortTextEditor);
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "Session",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 4
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 30;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("Session", ShortTextEditor);
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "Condition",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 19
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.plceholder= "asasa";
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("Condition", ShortTextEditor);
////----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "DbTableName",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 18
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("DbTableName", ShortTextEditor);
////----------------------------------

SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "ValidationFiled",
        type: "text",
        isRequired: true,
        category: "general",
        visibleIndex: 20
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("ValidationFiled", ShortTextEditor);
//----------------------------------

SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "calltype",
        type: "text",
        isRequired: true,
        category: "API Type",
        visibleIndex: 2
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "text";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("Api", ShortTextEditor);
//----------------------------------
SurveyCreator
    .StylesManager
    .applyTheme("bootstrap");
Survey
    .JsonObject
    .metaData
    .addProperty("question", {
        name: "InternalApi",
        type: "boolean",
        isRequired: true,
        category: "API Type",
        visibleIndex: 3
    });

var ShortTextEditor = {
    render: function (editor, htmlElement) {
        var input = document.createElement("input");
        input.className = "form-control svd_editor_control";
        input.type = "boolean";
        input.maxLength = 5;
        input.style.width = "100%";
        htmlElement.appendChild(input);
        input.onchange = function () {
            editor.koValue(input.value);
        }
        editor.onValueUpdated = function (newValue) {
            input.value = editor.koValue() || "";
        }
        input.value = editor.koValue() || "";
    }
};

SurveyCreator
    .SurveyPropertyEditorFactory
    .registerCustomEditor("InternalApi", ShortTextEditor);
//----------------------------------

//var creatorOptions = {};
//var creator = new SurveyCreator.SurveyCreator("creatorElement", creatorOptions);
//creator.showToolbox = "right";
//creator.showPropertyGrid = "right";
//creator.rightContainerActiveItem("toolbox");
Survey.surveyLocalization.locales[Survey.surveyLocalization.defaultLocale].requiredError = "Required";
surveyCreator
    .toolbox
    .changeCategories([
        {
            name: "panel",
            category: "Panels"
        }, {
            name: "paneldynamic",
            category: "Panels"
        }, {
            name: "matrix",
            category: "Matrix"
        }, {
            name: "matrixdropdown",
            category: "Matrix"
        }, {
            name: "matrixdynamic",
            category: "Matrix"
        }
    ]);
surveyCreator
    .toolbox
    .addItem({
        name: "countries",
        isCopied: true,
        iconName: "icon-default",
        title: "Odisha State",
        category: "Custom",
        json:
        {
            "type": "panel", "name": "panel1",
            "elements": [
                {
                    "type": "dropdown", "name": "District", "title": "District", "choicesByUrl": { "url": "http://lgdmapping.csmpl.com/api/DU/Getdistricts", "valueName": "districT_CODE", "titleName": "districT_NAME" }
                }, {
                    "type": "dropdown", "name": "Blocks", "title": "Blocks", "choicesByUrl": { "url": "http://lgdmapping.csmpl.com/api/DU/Getblocks/{District}", "valueName": "blocK_CODE", "titleName": "blocK_NAME" }, "choicesVisibleIf": "{District} notempty", "choicesEnableIf": "{District} notempty"
                }, {
                    "type": "dropdown", "name": "GramPanchayat", "title": "Gram Panchayat", "choicesByUrl": { "url": "http://lgdmapping.csmpl.com/api/DU/getGP/{Blocks}", "valueName": "gP_CODE", "titleName": "gP_NAME" }, "choicesVisibleIf": "{Blocks} notempty", "choicesEnableIf": "{Blocks} notempty"
                }, {
                    "type": "dropdown", "name": "Village", "title": "Village", "choicesByUrl": { "url": "http://lgdmapping.csmpl.com/api/DU/getVillages/{GramPanchayat}", "valueName": "villagE_CODE", "titleName": "villagE_NAME" }, "choicesVisibleIf": "{GramPanchayat} notempty", "choicesEnableIf": "{GramPanchayat} notempty"
                }],
            "title": "Odisha State"
        }
    });
//----------------------------------
surveyCreator
    .toolbox
    .addItem({
        name: "year",
        isCopied: true,
        iconName: "icon-default",
        title: "Year Month",
        category: "Custom",
        json:
        {
            "type": "panel", "name": "panel2",
            "elements": [
                {
                    "type": "dropdown", "name": "Year", "title": "Year", "choicesByUrl": { "url": "http://lgdmapping.csmpl.com/api/DU/Getdistricts", "valueName": "districT_CODE", "titleName": "year" }
                }, {
                    "type": "dropdown", "name": "Month", "title": "Month", "choices": [ {
                        "value": "January",
                        "text": "January"
                },
                {
                    "value": "February",
                    "text": "February"
                },
                {
                    "value": "March",
                    "text": "March"
                },
                {
                    "value": "April",
                    "text": "April"
                },
                {
                    "value": "May",
                    "text": "May"
                },
                {
                    "value": "June",
                    "text": "June"
                        },
                        {
                            "value": "July",
                            "text": "July"
                        },
                        {
                            "value": "August",
                            "text": "August"
                        },
                        {
                            "value": "September",
                            "text": "September"
                        },
                        {
                            "value": "October",
                            "text": "October"
                        },
                        {
                            "value": "November",
                            "text": "November"
                        },
                        {
                            "value": "December",
                            "text": "December"
                        }
                    ]
                }],
            "title": ""
        }
    });
//----------------------------------
surveyCreator.onQuestionAdded.add(function (sender, options) {
    //console.log(options.question); 
});

surveyCreator.onPanelAdded.add(function (sender, options) {   //fires only on adding Panel One Time
    if (options.panel.title === "Odisha State") {

        var district = options.panel.elements[0].name;
        options.panel.elements[1].choicesByUrl.url = "http://lgdmapping.csmpl.com/api/DU/Getblocks/{" + district + "}";
        options.panel.elements[1].choicesVisibleIf = "{" + district + "} notempty";
        options.panel.elements[1].choicesEnableIf = "{" + district + "} notempty";

        var block = options.panel.elements[1].name;
        options.panel.elements[2].choicesByUrl.url = "http://lgdmapping.csmpl.com/api/DU/getGP/{" + block + "}";
        options.panel.elements[2].choicesVisibleIf = "{" + block + "} notempty";
        options.panel.elements[2].choicesEnableIf = "{" + block + "} notempty";

        var gp = options.panel.elements[2].name;
        options.panel.elements[3].choicesByUrl.url = "http://lgdmapping.csmpl.com/api/DU/getVillages/{" + gp + "}";
        options.panel.elements[3].choicesVisibleIf = "{" + gp + "} notempty";
        options.panel.elements[3].choicesEnableIf = "{" + gp + "} notempty";
    }
});

surveyCreator.isAutoSave = true;
surveyCreator.showState = true;
surveyCreator.showOptions = true;

//add file type into matrix columns (run-time)
Survey.matrixDropdownColumnTypes.file = { properties: ["showPreview", "imageHeight", "imageWidth"] };
//add file type into matrix columns (design-time/editor)
SurveyCreator.SurveyQuestionEditorDefinition.definition["matrixdropdowncolumn@file"] = {
    properties: ["showPreview", "imageHeight", "imageWidth"],
    tabs: [{ name: "visibleIf", index: 12 }, { name: "enableIf", index: 20 }]
};
surveyName = decodeURI(getParams()["name"]);
setSurveyName(surveyName);
