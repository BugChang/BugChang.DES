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
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: { deviceCode: deviceCode },
            headers:
            {
                "BugChang-CSRF-HEADER": token //注意header要修改
            },
            url: "/Bill/CheckReceive",
            success: function (result) {
                if (result.success) {
                    if (result.data.length === 1) {
                        var objectId = result.data[0].id;
                        printReceive(objectId);
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
            }
        });
    }

    function checkSend() {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: { deviceCode: deviceCode },
            headers:
            {
                "BugChang-CSRF-HEADER": token //注意header要修改
            },
            url: "/Bill/CheckSend",
            success: function (result) {
                if (result.success) {
                    if (result.data.length === 1) {
                        var objectId = result.data[0].id;
                        printSend(objectId);
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
            }
        });
    }

    function checkReceiveSend() {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: { deviceCode: deviceCode },
            headers:
            {
                "BugChang-CSRF-HEADER": token //注意header要修改
            },
            url: "/Bill/CheckSendAndReceive",
            success: function (result) {
                if (result.success) {
                    printReceiveSend();
                } else {
                    window.toastr.error(result.message);
                }
            }
        });
    }

    function printReceive(objectId) {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: { objectId: objectId, deviceCode: deviceCode },
            headers:
            {
                "BugChang-CSRF-HEADER": token //注意header要修改
            },
            url: "/Bill/CreateReceiveBill",
            success: function (result) {
                if (result.success) {
                    location.href = "/Bill/Detail/" + result.data;
                } else {
                    window.toastr.error(result.message);
                }
            }
        });
    }

    function printSend(departmentId) {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: { departmentId: departmentId, deviceCode: deviceCode },
            headers:
            {
                "BugChang-CSRF-HEADER": token //注意header要修改
            },
            url: "/Bill/CreateSendBill",
            success: function (result) {
                if (result.success) {
                    location.href = "/Bill/Detail/" + result.data;
                } else {
                    window.toastr.error(result.message);
                }
            }
        });
    }


    function printReceiveSend() {
        $('body').loading();
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: { deviceCode: deviceCode },
            headers:
            {
                "BugChang-CSRF-HEADER": token //注意header要修改
            },
            url: "/Bill/CreateReceiveSendBill",
            success: function (result) {
                if (result.success) {
                    location.href = "/Bill/Detail/" + result.data;
                } else {
                    window.toastr.error(result.message);
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