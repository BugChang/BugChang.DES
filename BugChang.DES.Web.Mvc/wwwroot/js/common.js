var Common = function () {
    //toastr提示2s自动关闭
    window.toastr.options.timeOut = 2000;
    var isDevelopment = true;
    var operationCodes;

    // 设置jQuery Ajax全局的参数  
    $.ajaxSetup({
        error: function (jqXhr, textStatus, errorThrown) {
            switch (jqXhr.status) {
                case (500):
                    window.toastr.error("服务器错误，请联系相关负责人处理！");
                    break;
                case (401):
                    window.swal({
                        title: '您太久没有进行操作了，请重新登录',
                        //text: '删除后无法恢复数据!',
                        icon: 'info',
                        buttons: ['取消', '确定']
                    }).then((relogin) => {
                        if (relogin) {
                            location.href = '/Account/Login';
                        }
                    });
                    break;
                case (403):
                    window.toastr.error("权限不足！");
                    break;
                case (404):
                    window.toastr.error("请求地址不存在！");
                    break;
                case (408):
                    window.toastr.error("请求超时！");
                    break;
                default:
                    window.toastr.error("未知错误！");
            }
        }
    });

    //初始化操作列表
    function initOperations(module) {
        $.ajax({
            type: 'GET',
            async: false,
            data: { module: module },
            url: "/Role/GetRoleOperations",
            success: function (result) {
                operationCodes = result;
            }
        });
    }

    //判断是否具备操作权限
    function hasOperation(operationCode) {
        if (isDevelopment) {
            return true;
        }
        var flag = false;
        $.each(operationCodes, function (idx, obj) {
            if (obj === operationCode) {
                flag = true;
                return false;
            }
            return true;
        });
        return flag;
    }

    function alert(text) {
        window.swal({
            text: text,
            button: '关闭'
        });
    }

    //向外暴露方法
    return {
        alert: alert,
        initOperations: initOperations,
        hasOperation: hasOperation
    };
}();  