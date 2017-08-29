//验证字符串是否是正数字（带4位小数据点）格式
function IsPositiveNum(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /^\d+(\.\d{1,4})?$/; ///^[0-9]+(.[0-9]{1,4})?/;
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入数字格式：[ddd.dddd]。<br>";
            }
        if ($(item).val().indexOf("0") == 0 && $(item).val().indexOf(".") != 1 && $(item).val().length > 1) {
            errors += "[" + $(item).attr("title") + "] 请输入数字格式：[ddd.dddd]。<br>";
        }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

//验证字符串是否是正数字（带2位小数据点）格式
function IsPositiveNum2(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /^\d+(\.\d{1,2})?$/; ///^[0-9]+(.[0-9]{1,4})?/;
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入数字格式：[ddd.dd]。<br>";
            }
        if ($(item).val().indexOf("0") == 0 && $(item).val().indexOf(".") != 1 && $(item).val().length > 1) {
            errors += "[" + $(item).attr("title") + "] 请输入数字格式：[ddd.dd]。<br>";
        }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsDecimal18_2(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /^-?\d+(\.\d{1,2})?$/;
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入数字格式：[ddd.dd]。<br>";
            }
        if ($(item).val().indexOf("0") == 0 && $(item).val().indexOf(".") != 1 && $(item).val().length > 1) {
            errors += "[" + $(item).attr("title") + "] 请输入数字格式：[ddd.dd]。<br>";
        }
        if ($(item).val().indexOf("-") == 0 && $(item).val().indexOf("0") == 1 && $(item).val().indexOf(".") != 2 && $(item).val().length > 1) {
            errors += "[" + $(item).attr("title") + "] 请输入数字格式：[-ddd.dd]。<br>";
        }
    });
    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsInt(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /^-?[1-9]\d{0,8}$/; //^-?[1-9]\d*$
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入整型数字格式。<br>";
            }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsId(id) {

    var errors = "";
    var regex = /\d{17}[\d|X]|\d{15}/; //^-?[1-9]\d*$
    if ($("#" + id).val() != "")
        if (!regex.test($("#" + id).val())) {
            errors += "[" + $("#" + id).attr("title") + "] 请输入正确身份证号。<br>";
        }

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsTel(id) {

    var errors = "";
    var regex = /(^(\d{3,4}-)?\d{7,8})$/; //^-?[1-9]\d*$
    if ($("#" + id).val() != "")
        if (!regex.test($("#" + id).val())) {
            errors += "[" + $("#" + id).attr("title") + "] 请输入正确的区号-号码的座机号信息。<br>";
        }

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsPhone(id) {

    var errors = "";
    var regex = /1\d{10}/; //^-?[1-9]\d*$
    if ($("#" + id).val() != "")
        if (!regex.test($("#" + id).val())) {
            errors += "[" + $("#" + id).attr("title") + "] 请输入正确的11位的手机号信息。<br>";
        }

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsRequired3(id) {

    var errors = "";
    if ($("#" + id).val() == "") {
        errors += "[" + $("#" + id).attr("title") + "] 不可为空。<br>";
    }

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsIntHasZero(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /((^(?!0)\d+)|(^(?=0)\d$))/; //^-?[1-9]\d*$ 
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入整型数字格式。<br>";
            }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsRequired(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        if ($(item).val() == "" && !$(item).attr("disabled")) {
            errors += "[" + $(item).attr("title") + "] 不可为空。<br>";
        }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function IsRequired2(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        if ($(item).val() == "") {
            errors += "[" + $(item).attr("title") + "] 不可为空。<br>";
        }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}


//批号验证,8位数字
function IsLot(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /^[0-9]\d{7}$/;
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入8位数字：[dddddddd]。<br>";
            }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

//特殊字符验证
function IsSpecialChar(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】‘；：”“'。，、？]");
        if ($(item).val() != "")
            if (regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 存在特殊字符，请更正。<br>";
            }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

//库位验证,8位数字
function IsCargoCode(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        var regex = /^[0-9]\d{7}$/;
        if ($(item).val() != "")
            if (!regex.test($(item).val())) {
                errors += "[" + $(item).attr("title") + "] 请输入8位数字：[dddddddd]。<br>";
            }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function NoZero(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        if ($(item).val() != "" && $(item).val() == "0") {
            errors += "[" + $(item).attr("title") + "] 不可为0。<br>";
        }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}

function NoId(className) {

    var errors = "";
    $(className).each(function (idx, item) {
        if ($(item).val() != "" && $(item).val() == "0") {
            errors += "[" + $(item).attr("title") + "] 不可为0。<br>";
        }
    });

    if (errors != "") {
        ShowErrorsMsgDialog(errors, "no", false);
        return false;
    }
    return true;
}