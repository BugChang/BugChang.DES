(function () {
    var socket;
    var deviceCode;
    $(function () {

        initSocket();

        $('input').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue',
            increaseArea: '20%' /* optional */
        });
        $('form').submit(function () {
            $("#Password").val(hex_md5($("#Password").val()));
            $('body').loading();
        });
    });

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
                $.post("/Accoount/LoginWithCard",
                    {
                        cardNo: obj.Data,
                        deviceCode: deviceCode
                    },
                    function (result) {
                        if (result.success) {
                            location.href = result.data;
                        } else {
                            window.toastr.error(result.message);
                        }
                    });
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