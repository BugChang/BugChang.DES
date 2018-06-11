$(function () {
    //toastr提示2s自动关闭
    window.toastr.options.timeOut = 2000;
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
});  