(function () {
    var socket;
    var deviceCode;
    var maxTime = 20;
    var time = maxTime;
    $(function () {
        initSocket();

        $('body').on('keydown mousemove', function (e) {
            time = maxTime; // reset
        });

        var intervalId = setInterval(function () {
            time--;
            if (time <= 0) {
                logOut();
                clearInterval(intervalId);
            }
        },
            1000);

        $("#btnPrintAndExit").click(function () {
            print("/Account/Logout");
        });


        $("#btnPrintAndReturn").click(function () {
            print("/Bill/Index");
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

            var getSerialPortList = { command: 'GetSerialPortList' };
            socket.send(JSON.stringify(getSerialPortList));
        };
        socket.onmessage = function (e) {
            var obj = JSON.parse(e.data);
            if (obj.Method === "GetMacAddress") {
                deviceCode = obj.Data;
            }
        };
        socket.onerror = function (e) {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }

    function print(href) {
        $.get("/HardWare/GetLaserPrintA4",
            { deviceCode: deviceCode },
            function (data) {
                var lodop = getLodop();
                lodop.PRINT_INIT("");
                var strStyle =
                    "<style> table,td,th {border-width: 1px;border-style: solid;border-color:black;border-collapse: collapse;line-height:30px}</style>";
                lodop.ADD_PRINT_TABLE(180,
                    "5%",
                    "90%",
                    314,
                    strStyle + document.getElementById("divBody").innerHTML);
                lodop.SET_PRINT_STYLEA(0, "Vorient", 3);
                lodop.ADD_PRINT_HTM(20, "5%", "90%", 109, document.getElementById("divHeader").innerHTML);
                lodop.SET_PRINT_STYLEA(0, "ItemType", 1);
                lodop.SET_PRINT_STYLEA(0, "LinkedItem", 1);
                lodop.ADD_PRINT_HTM(500, "5%", "90%", 54, document.getElementById("divFooter").innerHTML);
                lodop.SET_PRINT_STYLEA(0, "ItemType", 1);
                lodop.SET_PRINT_STYLEA(0, "LinkedItem", 1);
                lodop.SET_PRINTER_INDEX(data.value);
                lodop.PRINT();
                setTimeout('location.href="' + href + '"', 1000);
            });
    }

    function logOut() {
        window.location.href = "/Account/Logout";
    }
})();