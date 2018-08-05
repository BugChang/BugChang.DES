(function () {
    var macAddress;
    var socket;
    $(function () {

        initSocket();

        $("#btnPrintDifferent").click(function () {
            printDifferent();
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
                macAddress = obj.Data;
                openScanGun();
            } else if (obj.Method === "OpenSerialPort") {
                if (obj.Data) {
                    window.toastr.success("扫描枪已就绪");
                } else {
                    window.toastr.error("连接扫描枪出现错误");
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

    function openScanGun() {
        console.log(macAddress);
        if (macAddress !== undefined) {
            $.post("/HardWare/GetScanGun", { deviceCode: macAddress }, function (data) {
                if (data === null || data === undefined) {
                    window.toastr.error("硬件参数获取失败，请确认本机硬件参数设置是否正确！");
                } else {
                    var row = { command: 'OpenSerialPort', serialPortName: data.value, baudRate: data.baudRate };
                    socket.send(JSON.stringify(row));
                }
            });
        } else {
            window.toastr.error("设备码获取失败，请检查扫描枪设置！");
        }
    }

    function printDifferent() {
        $.post("/HardWare/GetLaserPrintA4",
            { deviceCode: macAddress },
            function (data) {
                if (data === null || data === undefined) {
                    window.toastr.error("硬件参数获取失败，请确认本机硬件参数设置是否正确！");
                } else {
                    window.toastr.success("正在打印，请稍等...");
                    var barcodeNo = $("#barcodeNo").val();
                    var lodop = getLodop();
                    lodop.PRINT_INIT("");
                    lodop.ADD_PRINT_TEXT("24.34mm", "3.7mm", "185.21mm", "12.17mm", "异形件通知单");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 24);
                    lodop.SET_PRINT_STYLEA(0, "Alignment", 2);
                    lodop.SET_PRINT_STYLEA(0, "Bold", 1);
                    lodop.ADD_PRINT_BARCODE("4.76mm",
                        "63.24mm",
                        "69.59mm",
                        "16.14mm",
                        "128Auto",
                        barcodeNo);
                    lodop.SET_PRINT_STYLEA(0, "ShowBarText", 0);
                    lodop.ADD_PRINT_TEXT("49.74mm", "24.61mm", "151.87mm", "22.49mm", "您有一份异形件，条码编号为:" + getLetterNo(barcodeNo) + "，请凭此单与管理人员联系取件。");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 15);
                    lodop.ADD_PRINT_TEXT("111.92mm", "123.83mm", "39.42mm", "5.29mm", "领取人签字：");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
                    lodop.ADD_PRINT_TEXT("123.3mm", "123.3mm", "27.78mm", "5.29mm", "日期：");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
                    lodop.SET_PRINTER_INDEX(data.value);
                    lodop.PRINT();
                }
            });
    }

    function getLetterNo(barcodeNo) {
        if (barcodeNo.length === 33) {
            return barcodeNo.substring(15, 22);
        } else if (barcodeNo.length === 26) {
            return barcodeNo.substring(8, 15);
        } else {
            return barcodeNo;
        }
    }
})();