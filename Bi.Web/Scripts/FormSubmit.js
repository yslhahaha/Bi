//
function Submit() {
    var arguCount = arguments.length;

    if (arguCount == 5) { FormSubmit(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]); }
    if (arguCount == 6) { FormSubmitAAC(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]); }
    if (arguCount == 7) { FormSubmitABAC(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]); }
}

//新建页面时提交表单，重置表单数据（清空）AndBeforeAfterCall
function FormSubmitABAC(btn, form, url, msg, reset, beforeFun, afterFun) {
    $("#" + btn).unbind();
    $("#" + btn).click(function () {
        if (!beforeFun()) return;
        $.ajax({
            cache: false,
            type: "POST",
            url: url,
            data: $('#' + form).serialize(),
            beforeSend: function () {
                Loading(true);
            },
            success: function (success) {
                Loading(false);
                if (reset) { ResetForm(form); }
                ShowMsg(success, msg);
            },
            error: function (xhr, status, error) {
                Loading(false);
                var errs = ParseExceptionMsg(xhr);
                ShowErrorsMsgDialog(errs, "no", false);
            },
            complete: function () {
                Loading(false);
                afterFun(false);
            }
        });
    });
}

function ShowMsg(isOk, msg) {
    if (isOk == "ok") {
        $('#msgArea').after(
            '<div id="showMsg" role="alert" class="col-md-12 margin-bottom-15 alert alert-success glyphicon glyphicon-ok">' +
              '<a href="#" class="close" data-dismiss="alert">&times;</a>' +
              '&nbsp;<strong>已完成，</strong>' + msg + '。' + '</div>');
        setTimeout(function () {
            $('#showMsg').remove();
        }, 5000);
    } else {

    }
}

function ShowErrTip(ctrl, msg) {

    $(ctrl).popover({
        "html": true,
        "content": msg,
        "placement": "right",
        "trigger": "manual",
        "template": '<div class="popover" role="tooltip"><div class="arrow"></div><div class="popover-content" style="width:200px;"></div></div>'
    });

    $(ctrl).popover("show").on("click", function () {
        $(ctrl).popover("hide");
    });
}

//显示前台验证错误..等信息的弹出对话框
function ShowErrorsMsgDialog(msg, flag, refresh) {
    $("#errMsg").html(msg);
    $('#errModal').modal('show');
}

function Loading(isShow) {
    if (isShow) {
        $('#loading').modal('show');
    } else {
        $('#loading').modal('hide');
    }
}

//Ajax请求
function AjaxRequest(Action, parms, afterFun) {
    $.ajax({
        type: "POST", //调用Action使用Post方式请求
        contentType: "application/json;charset=utf-8", //Action 会返回Json类型
        url: Action, //调用Action    /Manage/User/Create
        data: parms, //参数 {"taslyid":"AT1141","password":"123456"}
        dataType: 'json',
        beforeSend: function (x) { x.setRequestHeader("Content-Type", "application/json; charset=utf-8"); },
        error: function (xhr, status, error) {
            var errs = ParseExceptionMsg(xhr); //Pub.js
            ShowMsgDialog(errs, "no", false);
        },
        success: function (result) {
            afterFun(result);
        }
    });
}

/// <summary>
/// 重置表单
/// </summary>
/// <param name="type">重置类型，新增和编辑（new,edit）</param>
/// <param name="datasource">数据源（json格式的字符串,编辑时，存放于前台hid中的初始化数据）</param>
/// <param name="usable">控件锁定</param>
/// <param name="prefixName">控件前缀名</param>
function ResetForm(form) {
    $('#' + form)[0].reset();
}


//解析Ajax调用Controller方法时，返回的错误信息，可以是一条信息，也要是Json格式的信息
//组织要显示错误信息
function ParseExceptionMsg(xhr) {
    var jsonValue = $.parseJSON(xhr.responseText);
    var errs = "";
    if (IsJsonString(jsonValue.ErrorMessage)) {
        var errorMessage = $.parseJSON(jsonValue.ErrorMessage);
        $.each(errorMessage, function (name, value) {
            errs += "[" + $("label[for='" + name + "']").text() + "]:";
            errs += value + "<br/>";
        });
    } else {
        errs = jsonValue.ErrorMessage;
    }

    return errs;
}