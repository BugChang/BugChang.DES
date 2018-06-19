var Common = function () {
    //toastr提示2s自动关闭
    window.toastr.options.timeOut = 2000;

    var operationCodes;

    // 设置jQuery Ajax全局的参数  
    $.ajaxSetup({
        error: function (jqXhr, textStatus, errorThrown) {
            switch (jqXhr.status) {
                case (500):
                    window.toastr.error("服务器出现了一个错误，请联系管理员进行维护！");
                    break;
                case (401):
                    window.toastr.error("登录已过期，请重新登录！");
                    break;
                case (403):
                    window.toastr.error("无权限执行此操作！");
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

    //向外暴露方法
    return {
        initOperations: initOperations,
        hasOperation: hasOperation
    };
}();  