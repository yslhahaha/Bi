function AjaxGetCall(url) {
    $.ajax({
        cache: false,
        type: "GET",
        url: url,
        beforeSend: function () {
            Loading(true);
        },
        success: function (res) {
            Loading(false);
            return res;
        },
        error: function (xhr, status, error) {
            Loading(false);
            var errs = ParseExceptionMsg(xhr);
            ShowErrorsMsgDialog(errs, "no", false);
        },
        complete: function () {
            Loading(false);
        }
    });
}



//弹出窗口
function ClientWindow(params) {
    //打开方式：[Jdialog]、[模式对话框]
    this.open = (params.open || "Jdialog");
    //信息类型：[页面]、[消息]、[其它]
    this.type = (params.type || "msg");
    //窗口ID
    this.id = (params.id || "openWindow");
    //窗口标题
    this.title = (params.title || "openWindow");
    //窗口内网页URL
    this.url = (params.url || "");
    this.width = (params.width || "400");
    this.height = (params.height || "400");
    this.msg = (params.msg || "")
}

Date.prototype.format = function (format) {
    /* 
    * eg:format="yyyy-MM-dd hh:mm:ss"; 
    */
    var o = {
        "M+": this.getMonth() + 1, // month  
        "d+": this.getDate(), // day  
        "h+": this.getHours(), // hour  
        "m+": this.getMinutes(), // minute  
        "s+": this.getSeconds(), // second  
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter  
        "S": this.getMilliseconds()
        // millisecond  
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4
                    - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1
                        ? o[k]
                        : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}


////获取页面Url参数值
function GetUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]);
    return null; //返回参数值
}

//判断是否是Json的String类型
function IsJsonString(data) {
    if (!data.match("^\{(.+:.+,*){1,}\}$")) {
        return false;
    }
    else {
        return true;
    }
}

//千分位
function isString(num) {
    return Object.prototype.toString.apply(num) == '[object String]';
}
function fmoney(s, n) {
    n = n > 0 && n <= 20 ? n : 2;

    if (isString(s)) {
        s = s * 1;
    }
    s = parseFloat((s + "")).toFixed(n) + "";

    var l = s.split(".")[0].split(""),
        r = s.split(".")[1];

    t = "";
    for (i = 0; i < l.length; i++) {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
    }
    return t.split("").join("") + "." + r;
}

function fmoneyBackToNumber(num) {
    var x = num.split(',');
    return parseFloat(x.join(""));
}

//查找在Grid页面里，ID为hidColsID这个参数传递的隐藏控件
//在hidColsID控件中，放有当前页面所有列的名称与对应的Idx
//在GridPartil.cshtml页面有hidColsID指定的控件
function GetGridColIndexByColName(hidColsID, colName) {
    var colidx;

    var gridCols = $.parseJSON($("#" + hidColsID).val());

    $.each(gridCols, function (idx, item) {
        if (item.cName == colName) {
            colidx = item.colIdx;
            return false;
        }
    });

    return colidx;
}

//模糊层
function BlurMark(open) {
    if (open) $("body").addClass("modal-active");
    else $("body").removeClass("modal-active");
}

//为需要的标签显示Tip,提示框
function CreateTip() {
    $('.tip').miniTip({
        'contentAttribute': 'data-tip',
        'className': 'blue',
        'offset': 20,
        'showAnimateProperties': { 'top': '-=10' },
        'hideAnimateProperties': { 'top': '+=10' },
        'position': 'bottom',
        'onLoad': function (element, tooltip) {
            $(element).animate({ 'opacity': 0.35 }, '350');

        },
        'onHide': function (element, tooltip) {
            $(element).animate({ 'opacity': 1 }, '250');

        }
    });
}