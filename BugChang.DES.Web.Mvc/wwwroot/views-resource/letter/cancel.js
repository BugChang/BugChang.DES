(function () {
    var macAddress;
    var socket;
    var cancelTable;
    var searchTable;
    $(function () {

        initSocket();

        initCancelTable();

        initSearchTable();

        //initApplicants();

        $("#btnSearch").click(function () {
            var barcodeNo = $("#barcodeNo").val();
            if (!barcodeNo) {
                window.toastr.error("条码号不能为空！");
            } else {
                searchTable.draw(false);
                $("#SearchBarcodeModal").modal('show');
            }
        });

        $("#btnSave").click(function () {
            var count = searchTable.rows({ selected: true }).count();
            if (count === 0) {
                window.toastr.error("请选择要勘误的记录");
            } else {
                var letterId = searchTable.rows({ selected: true }).data()[0].id;

                var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
                $.ajax({
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: { id: letterId},
                    headers:
                        {
                            "BugChang-CSRF-HEADER": token //注意header要修改
                        },
                    url: "/Letter/CancelLetter/",
                    success: function (result) {
                        if (result.success) {
                            window.toastr.success("勘误成功");
                            cancelTable.draw(false);
                        } else {
                            window.toastr.error(result.message);
                        }
                    }
                });
            }
        });

        $('#cancelTable').delegate('.detail-log',
            'click',
            function () {
                var letterId = $(this).attr('data-letter-id');
                exchangeLogs(letterId);
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
            } else {
                $("#barcodeNo").val(obj.Data);
            }
        };
        socket.onerror = function () {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }

    function openScanGun() {
        console.log(macAddress);
        if (macAddress !== undefined) {
            $.get("/HardWare/GetScanGun", { deviceCode: macAddress }, function (data) {
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

    function initApplicants() {
        $.get('/Letter/GetUsers',
            function (data) {
                $('#applicantId').select2({
                    data: data,
                    allowClear: false
                });
            });
    }

    function initCancelTable() {
        cancelTable = $('#cancelTable').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: '/Letter/GetCancelLetters'
            },
            stateSave: true,
            columns: [
                {
                    data: 'barcodeNo',
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
                },
                {
                    data: 'createUserName',
                    title: '创建人'
                },
                {
                    data: 'createTime',
                    title: '创建时间'
                },
                {
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    render: function (data, type, row) {

                        return row.barcodeNo.substring(15, 22);
                    }
                },
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
                },
                {
                    targets: 13,
                    render: function (data, type, row) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-primary btn-xs detail-log" data-letter-id=' + row.id + '>详情日志</button>&nbsp;';
                        return strHtml;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

    function initSearchTable() {
        searchTable = $('#searchTable').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            select: true,
            ajax: {
                url: '/Letter/SearchCancelLetters/',
                data: function (d) {
                    d.letterNo = $("#barcodeNo").val();
                }
            },

            stateSave: true,
            columns: [

                {
                    data: 'barcodeNo',
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
                },
                {
                    data: 'createUserName',
                    title: '创建人'
                },
                {
                    data: 'createTime',
                    title: '创建时间'
                }
            ],
            columnDefs: [
                {
                    targets: 0,
                    render: function (data, type, row) {

                        return row.barcodeNo.substring(15, 22);
                    }
                },
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

    //流转详情
    function exchangeLogs(letterId) {
        $('#ExchangeLogModal .modal-content').load('/Letter/ExchangeLog/' + letterId);
        $('#ExchangeLogModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }
})();