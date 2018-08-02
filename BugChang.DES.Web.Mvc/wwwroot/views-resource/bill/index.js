(function () {
    var socket;
    var deviceCode;
    $(function () {

        initSocket();

        $(".print-receive").click(function () {
            checkReceive();
        });
        $(".print-send").click(function () {
            checkSend();
        });

        $(".print-receive-send").click(function () {
            checkReceiveSend();
        });

        $('#SelectObjectModal .modal-body').delegate('.print-receive-detail',
            'click',
            function () {
                $("#SelectObjectModal").modal("hide");
                var objectId = $(this).attr('data-object-id');
                printReceive(objectId);
            });

        $('#SelectObjectModal .modal-body').delegate('.print-send-detail',
            'click',
            function () {
                $("#SelectObjectModal").modal("hide");
                var departmentId = $(this).attr('data-department-id');
                printSend(departmentId);
            });
    });

    function checkReceive() {
        $.post("/Bill/CheckReceive",
            { deviceCode: deviceCode },
            function (result) {
                if (result.success) {
                    if (result.data.length === 1) {
                        alert("就一个直接打印吧");
                    } else {
                        $("#SelectObjectModal .modal-body").html("");
                        var html = '<div class="row">';
                        $.each(result.data, function (idx, obj) {
                            html += '<div class="col-md-4">';
                            html += '<a class="btn btn-primary btn-block print-receive-detail" data-object-id=' + obj.id + '>' + obj.text + '</a>';
                            html += '</div >';
                        });
                        html += '</div >';
                        $("#SelectObjectModal .modal-body").html(html);
                        $("#SelectObjectModal").modal("show");
                    }

                } else {
                    window.toastr.error(result.message);
                }
            });
    }

    function checkSend() {
        $.post("/Bill/CheckSend",
            { deviceCode: deviceCode },
            function (result) {
                if (result.success) {
                    if (result.data.length === 1) {
                        alert("就一个直接打印吧");
                    } else {
                        $("#SelectObjectModal .modal-body").html("");
                        var html = '<div class="row">';
                        $.each(result.data, function (idx, obj) {
                            html += '<div class="col-md-4">';
                            html += '<a class="btn btn-primary btn-block print-send-detail" data-department-id=' + obj.id + '>' + obj.text + '</a>';
                            html += '</div >';
                        });
                        html += '</div >';
                        $("#SelectObjectModal .modal-body").html(html);
                        $("#SelectObjectModal").modal("show");
                    }

                } else {
                    window.toastr.error(result.message);
                }
            });
    }

    function checkReceiveSend() {

    }

    function printReceive(objectId) {
        $('body').loading();
        setTimeout('location.href = "/Account/Logout"', 5000);
    }

    function printSend(departmentId) {
        $('body').loading();
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
})();