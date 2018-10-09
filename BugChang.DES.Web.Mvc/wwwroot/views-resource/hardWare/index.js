(function () {
    var socket;
    var lodop;
    var macAddress;
    $(function () {
        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        initSocket();
        setTimeout(bindPrintList, 1000);

        $("#HardWareSaveForm").submit(function (e) {
            e.preventDefault();
            var data = JSON.stringify($(this).serialize());
            var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
            $.ajax({
                type: 'POST',
                async: false,
                cache: false,
                data: { postData: data },
                headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
                url: "/HardWare/Save",
                success: function (result) {
                    if (result.success) {
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                }
            });
        });
    });
    //绑定打印机列表
    function bindPrintList() {
        lodop = getLodop();
        for (var i = 0; i < $(".printer").length; i++) {
            lodop.Create_Printer_List($(".printer")[i]);
        }

        $.get("/HardWare/GeSettings?macAddress=" + macAddress, function (data) {
            var d;
            for (d in data) {
                if (data.hasOwnProperty(d)) {
                    if (data[d].baudRate) {
                        $("select[name=" + data[d].hardWareType + "BaudRate]").val(data[d].baudRate);
                    }
                    $("select[name=" + data[d].hardWareType + "]").val(data[d].value);
                }
            }
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

            var getSerialPortList = { command: 'GetSerialPortList' };
            socket.send(JSON.stringify(getSerialPortList));
        };
        socket.onmessage = function (e) {
            var obj = JSON.parse(e.data);
            if (obj.Method === "GetMacAddress") {
                macAddress = obj.Data;
                $("#macAddress").val(macAddress);
            } else if (obj.Method === "GetSerialPortList") {
                for (var j = 0; j < obj.Data.length; j++) {
                    $(".serialPort").append("<option value='" + obj.Data[j] + "'>" + obj.Data[j] + "</option>");
                }
            }
        };
        socket.onerror = function (e) {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }
})();