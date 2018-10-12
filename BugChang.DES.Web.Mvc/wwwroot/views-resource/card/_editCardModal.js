(function () {
    var socket;
    var deviceCode;

    $(function () {
        initSocket();

        initUsers();

        $("#CardEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Card/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#CardEditModal").modal("hide");
                        //刷新父页面
                        CardIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });
    //初始化用户下拉列表
    function initUsers() {
        $.get('/Card/GetUsers',
            function (data) {
                $('.edit-user-select').select2({
                    data: data,
                    allowClear: false
                });
                var userId = $("#DefaultValue").attr("data-user-id");
                $(".edit-user-select").val(userId).trigger("change");
            });
    }

    function initSocket() {
        if (typeof (WebSocket) === "undefined") {
            window.toastr.error("您的浏览器不支持WebSocket");
        }

        socket = new WebSocket("ws://localhost:8181");

        socket.onopen = function () {
            var getMacAddress = { command: 'GetMacAddress' };
            socket.send(JSON.stringify(getMacAddress));
        };
        socket.onmessage = function (e) {
            var obj = JSON.parse(e.data);
            if (obj.Method === "GetMacAddress") {
                deviceCode = obj.Data;
                $("#DeviceCode").val(deviceCode);
                openCpuCom();
            }
            if (obj.Method === "GetCpuCardNo") {
                if (obj.data === "") {
                    return false;
                }
                var d1 = obj.Data.substr(0, 2);
                var d2 = obj.Data.substr(2, 2);
                var d3 = obj.Data.substr(4, 2);
                var d4 = obj.Data.substr(6, 2);
                var cardValue = d4 + d3 + d2 + d1;
                $("#addValue").val(cardValue);
            }

        };
        socket.onerror = function (e) {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }

    function openCpuCom() {
        $.get("/HardWare/GetCpuReadCard",
            { deviceCode: deviceCode },
            function (data) {
                var row = { command: 'OpenCpuCom', port: data.value.replace("COM", ""), rate: data.baudRate };
                socket.send(JSON.stringify(row));
            });
    }
})();