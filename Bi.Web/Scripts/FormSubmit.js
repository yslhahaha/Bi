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
    $("#showMsg").remove();
    if (isOk == "ok") {
        $('#msgArea').after(
            '<div id="showMsg" role="alert" class="col-md-12 margin-bottom-15 alert alert-success glyphicon glyphicon-ok">' +
              '<a href="#" class="close" data-dismiss="alert">&times;</a>' +
              '&nbsp;<strong>操作成功，</strong>' + msg + '。' + '</div>');
        setTimeout(function () {
            $('#showMsg').remove();
        }, 5000);
    } else {
        $('#msgArea').after(
            '<div id="showMsg" style="margin-top:5px;" role="alert" class="col-md-12 margin-bottom-15 alert alert-warning alert-dismissible glyphicon glyphicon-alert">' +
              '<a href="#" class="close" data-dismiss="alert">&times;</a>' +
              '&nbsp;<strong>操作提示：</strong>' + isOk + msg + '。' + '</div>');
        setTimeout(function () {
            $('#showMsg').remove();
        }, 5000);
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

function AddLoading(obj) {
    if (obj)
        $(obj).html("<img id='imgLoading' src='/content/images/loader.gif' />");
    else
        $("#imgLoading").remove();
}

//Query页面，Ajax操作函数，
//返回结果（删除，锁定，可用，审核，退审等功能）
//url：ajax请求的Url，sMsg：操作成功后的显示信息，obj：操作控件
function DoAjaxAction(url, sMsg, grid) {
    $.ajax({
        type: "Post",
        url: url,
        cache: false,
        beforeSend: function () {
            Loading(true);
        },
        error: function (xhr, status, error) {
            var jsonValue = $.parseJSON(xhr.responseText)
            //alert(jsonValue.ErrorMessage);
            Loading(false);
            ShowMsg(jsonValue.ErrorMessage, " 操作失败");
        },
        success: function (success) {
            Loading(false);
            if (success == "ok") {
                ShowMsg(success, sMsg);
                $(grid).bs_grid('displayGrid', false);
            } else {
                ShowErrorsMsgDialog(success, "", "");
            }
        },
        complete: function () {
            Loading(false);
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