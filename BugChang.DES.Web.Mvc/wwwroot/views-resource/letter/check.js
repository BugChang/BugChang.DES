(function () {
    var macAddress;
    var socket;
    var table;
    var initFlag = 0;
    $(function () {

        initSocket();

        $('.search-time').datetimepicker({
            format: 'yyyy-mm-dd',
            language: 'zh-CN',
            autoclose: true,
            minView: 2
        });

        $("#CheckForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
            $.ajax({
                type: 'POST',
                async: false,
                cache: false,
                data: data,
                headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
                url: "/Letter/Check",
                success: function (result) {
                    if (result.success) {
                        $("#divInfo").html("");
                        var html = '<ul>';
                        for (var i = 0; i < result.data.length; i++) {
                            html += '<li>' + result.data[i] + '</li>';
                        }
                        html += '</ul>';
                        $("#divInfo").html(html);
                        if (initFlag===0) {
                            initTable();
                        } else {
                            table.ajax.reload();
                        }
                        
                    } else {
                        window.toastr.error(result.message);
                    }
                }
            });

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
                $("#DeviceCode").val(macAddress);
            }
        };
        socket.onerror = function () {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }


    function initTable() {
        initFlag = 1;
        table = $('#detailTable').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: '/Letter/GetCheckLetters',
                data: function (para) {
                    //添加额外的参数传给服务器
                    para.deviceCode = macAddress;
                    para.beginTime = $("#BeginTime").val();
                    para.endTime = $("#EndTime").val();
                }
            },
            stateSave: true,
            columns: [
                {
                    data: 'letterNo',
                    title: '信封编号'
                },
                {
                    data: 'oldBarcodeNo',
                    title: '原条码号'
                },
                {
                    data: 'receiveDepartmentName',
                    title: '收件单位'
                },
                {
                    data: 'receiver',
                    title: '收件人'
                },
                {
                    data: 'sendDepartmentName',
                    title: '发件单位'
                },
                {
                    data: 'oldSendDepartmentName',
                    title: '原发件单位'
                },
                {
                    data: 'secretLevel',
                    title: '秘密等级'
                },
                {
                    data: 'urgencyLevel',
                    title: '缓急程度'
                },
                {
                    data: 'urgencyTime',
                    title: '限时时间'
                },
                {
                    data: 'shiJiCode',
                    title: '市机码'
                },
                {
                    data: 'customData',
                    title: '附加数据'
                }
            ],
            columnDefs: [
                {
                    targets: 6,
                    render: function (data, type, row) {
                        var secretLevelText;
                        switch (row.secretLevel) {

                            case 0:
                                secretLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                secretLevelText = '<label class="label label-info"> 秘密</label>';
                                break;
                            case 2:
                                secretLevelText = '<label class="label label-warning">机密</label>';
                                break;
                            case 3:
                                secretLevelText = '<label class="label label-danger"> 绝密</label>';
                                break;
                            default:
                                secretLevelText = "未知";
                                break;
                        }
                        return secretLevelText;
                    }
                },
                {
                    targets: 7,
                    render: function (data, type, row) {
                        var secretLevelText;
                        switch (row.urgencyLevel) {

                            case 0:
                                secretLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                secretLevelText = '<label class="label label-info"> 紧急</label>';
                                break;
                            case 2:
                                secretLevelText = '<label class="label label-warning">特急</label>';
                                break;
                            case 3:
                                secretLevelText = '<label class="label label-danger"> 限时</label>';
                                break;
                            default:
                                secretLevelText = "未知";
                                break;
                        }
                        return secretLevelText;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

})();