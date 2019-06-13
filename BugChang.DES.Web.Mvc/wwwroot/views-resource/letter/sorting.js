(function () {
    var deviceCode;
    var socket;
    var tcjhTable = $("#tcjhTable");
    var jytxTable = $("#jytxTable");
    var zsTable = $("#zsTable");
    var jytxScanTable = $("#jytxScanTable");
    var tcjhListId;
    var zsListId;
    var jytxListId;
    var currentPage = 0;
    $(function () {

        initSocket();

        initTcjhTable();

        $("#btnTcjh").click(function () {
            currentPage = 0;
        });

        $("#btnJytx").click(function () {
            currentPage = 1;
            initJytxTable();
            intJytxScanTable();
        });

        $("#btnZs").click(function () {
            currentPage = 2;
            initZsRable();
        });

        $("#btnReloadTcjhTable").click(function () {
            tcjhTable.bootstrapTable('refresh', { silent: true });
        });

        $("#btnReloadJytxTable").click(function () {
            jytxListId = undefined;
            jytxTable.bootstrapTable('refresh', { silent: true });
            jytxScanTable.bootstrapTable('removeAll');
        });

        $("#btnReloadZsTable").click(function () {
            zsTable.bootstrapTable('refresh', { silent: true });
        });

        $('.tab-content').delegate('.change-jytx',
            'click',
            function () {
                var letterId = $(this).attr('data-letter-id');
                changeJytx(letterId);
            });

        $('.tab-content').delegate('.add-scan',
            'click',
            function () {
                var letterId = $(this).attr('data-letter-id');
                doSort(letterId);
            });

        $('.tab-content').delegate('.remove-scan',
            'click',
            function () {
                var letterId = $(this).attr('data-letter-id');
                doUnSort(letterId);
            });

        $("#btnPrintTcjhBill").click(function () {
            createTcjhList("printTcjhBill");
        });

        $("#btnWriteCpuCard").click(function () {
            createTcjhList("writeCpuCard");
        });


        $("#btnPrintZsBill").click(function () {
            createZsList();
        });

        $("#btnPrintJytxBill").click(function () {
            createJytxList();
        });


    });

    //初始化WebSocket
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
                openScanGun();
            } else if (obj.Method === "OpenSerialPort") {
                if (obj.Data) {
                    window.toastr.success("扫描枪已就绪");
                } else {
                    window.toastr.error("连接扫描枪出现错误");
                }
            } else if (obj.Method === "WriteCpuCard") {
                if (obj.Data) {
                    window.toastr.success("写卡成功");
                } else {
                    window.toastr.error("写卡失败");
                }
            } else if (obj.Method === "SerialPortReceived") {
                if (currentPage === 1) {
                    $.get("/Letter/GetLetterIdByBarcodeNo",
                        { barcodeNo: obj.Data },
                        function (letterId) {
                            if (letterId === 0) {
                                window.toastr.error("扫描失败！信件不存在");
                            } else {
                                doSort(letterId);
                            }

                        });
                }
            }

        };
        socket.onerror = function () {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }

    //打开扫描枪端口
    function openScanGun() {
        if (deviceCode !== undefined) {

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
                url: "/HardWare/GetScanGun",
                success: function (data) {
                    if (data === null || data === undefined) {
                        window.toastr.error("硬件参数获取失败，请确认本机硬件参数设置是否正确！");
                    } else {
                        var row = { command: 'OpenSerialPort', serialPortName: data.value, baudRate: data.baudRate };
                        socket.send(JSON.stringify(row));
                    }
                }
            });
        } else {
            window.toastr.error("设备码获取失败，请检查扫描枪设置！");
        }
    }

    //初始化同城交换表格
    function initTcjhTable() {
        tcjhTable.bootstrapTable({
            url: '/Letter/GetTcjhNoSortingLetters',
            method: 'get',
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            pageSize: 1000,
            paginationVAlign: 'bottom',
            clickToSelect: true,
            pagination: true,
            sidePagination: 'server',
            queryParams: function (params) {
                return {
                    search: params.search,
                    limit: params.limit,
                    offset: params.offset
                }
            },
            columns: [
                {
                    field: 'checkBox',
                    checkbox: true
                },
                {
                    field: 'number',
                    title: '序号',
                    formatter: function (value, row, index) {
                        return index + 1;
                    }
                }, {
                    field: 'letterNo',
                    title: '信封编号'
                }, {
                    field: 'receiveDepartmentName',
                    title: '收信单位'
                }, {
                    field: 'receiver',
                    title: '收件人'
                }, {
                    field: 'sendDepartmentName',
                    title: '发信单位'
                }, {
                    field: 'secretLevel',
                    title: '秘密等级',
                    formatter: function (value) {
                        var secretLevelText;
                        switch (value) {
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
                }, {
                    field: 'urgencyLevel',
                    title: '缓急程度',
                    formatter: function (value) {
                        var urgencyLevelText;
                        switch (value) {
                            case 0:
                                urgencyLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                urgencyLevelText = '<label class="label label-info"> 紧急</label>';
                                break;
                            case 2:
                                urgencyLevelText = '<label class="label label-warning">特急</label>';
                                break;
                            case 3:
                                urgencyLevelText = '<label class="label label-danger"> 限时</label>';
                                break;
                            default:
                                urgencyLevelText = "未知";
                                break;
                        }
                        return urgencyLevelText;
                    }
                }, {
                    field: 'urgencyTime',
                    title: '限时时间'
                }, {
                    field: 'id',
                    title: '操作',
                    formatter: function (value) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-warning btn-xs change-jytx" data-letter-id=' + value + '>转机要通信</button>';
                        return strHtml;
                    }
                }
            ]
        });
    }

    //初始化机要通信表格
    function initJytxTable() {
        jytxTable.bootstrapTable({
            url: '/Letter/GetJytxNoSortingLetters',
            method: 'get',
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            pageSize: 1000,
            paginationVAlign: 'bottom',
            clickToSelect: true,
            uniqueId: "id",
            pagination: true,
            sidePagination: 'server',
            queryParams: function (params) {
                return {
                    search: params.search,
                    limit: params.limit,
                    offset: params.offset
                }
            },
            columns: [
                {
                    field: 'number',
                    title: '序号',
                    formatter: function (value, row, index) {
                        return index + 1;
                    }
                },
                {
                    field: 'letterNo',
                    title: '信封编号'
                }, {
                    field: 'receiveDepartmentName',
                    title: '收信单位'
                }, {
                    field: 'sendDepartmentName',
                    title: '发信单位'
                }, {
                    field: 'secretLevel',
                    title: '秘密等级',
                    formatter: function (value) {
                        var secretLevelText;
                        switch (value) {
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
                }, {
                    field: 'urgencyLevel',
                    title: '缓急程度',
                    formatter: function (value) {
                        var urgencyLevelText;
                        switch (value) {
                            case 0:
                                urgencyLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                urgencyLevelText = '<label class="label label-info"> 紧急</label>';
                                break;
                            case 2:
                                urgencyLevelText = '<label class="label label-warning">特急</label>';
                                break;
                            case 3:
                                urgencyLevelText = '<label class="label label-danger"> 限时</label>';
                                break;
                            default:
                                urgencyLevelText = "未知";
                                break;
                        }
                        return urgencyLevelText;
                    }
                }, {
                    field: 'urgencyTime',
                    title: '限时时间'
                }, {
                    field: 'id',
                    title: '操作',
                    formatter: function (value) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-success btn-xs add-scan" data-letter-id=' + value + '>添加</button>';
                        return strHtml;
                    }
                }
            ]
        });
    }

    //初始化机要通信已扫描表格
    function intJytxScanTable() {
        jytxScanTable.bootstrapTable({
            url: '',
            method: 'get',
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            pageSize: 1000,
            paginationVAlign: 'bottom',
            clickToSelect: true,
            pagination: true,
            sidePagination: 'server',
            uniqueId: "id",
            queryParams: function (params) {
                return {
                    search: params.search,
                    limit: params.limit,
                    offset: params.offset
                }
            },
            columns: [
                {
                    field: 'number',
                    title: '序号',
                    formatter: function (value, row, index) {
                        return index + 1;
                    }
                },
                {
                    field: 'letterNo',
                    title: '信封编号'
                }, {
                    field: 'receiveDepartmentName',
                    title: '收信单位'
                }, {
                    field: 'sendDepartmentName',
                    title: '发信单位'
                }, {
                    field: 'secretLevel',
                    title: '秘密等级',
                    formatter: function (value) {
                        var secretLevelText;
                        switch (value) {
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
                }, {
                    field: 'urgencyLevel',
                    title: '缓急程度',
                    formatter: function (value) {
                        var urgencyLevelText;
                        switch (value) {
                            case 0:
                                urgencyLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                urgencyLevelText = '<label class="label label-info"> 紧急</label>';
                                break;
                            case 2:
                                urgencyLevelText = '<label class="label label-warning">特急</label>';
                                break;
                            case 3:
                                urgencyLevelText = '<label class="label label-danger"> 限时</label>';
                                break;
                            default:
                                urgencyLevelText = "未知";
                                break;
                        }
                        return urgencyLevelText;
                    }
                }, {
                    field: 'urgencyTime',
                    title: '限时时间'
                }, {
                    field: 'id',
                    title: '操作',
                    formatter: function (value) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-success btn-xs remove-scan" data-letter-id=' + value + '>移除</button>';
                        return strHtml;
                    }
                }
            ]
        });
    }

    //初始化直送表格
    function initZsRable() {
        zsTable.bootstrapTable({
            url: '/Letter/GetZsNoSortingLetters',
            method: 'get',
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            pageSize: 1000,
            paginationVAlign: 'bottom',
            clickToSelect: true,
            pagination: true,
            sidePagination: 'server',
            queryParams: function (params) {
                return {
                    search: params.search,
                    limit: params.limit,
                    offset: params.offset
                }
            },
            columns: [
                {
                    field: 'number',
                    title: '序号',
                    formatter: function (value, row, index) {
                        return index + 1;
                    }
                },
                {
                    field: 'checkBox',
                    checkbox: true
                }, {
                    field: 'letterNo',
                    title: '信封编号'
                }, {
                    field: 'receiveDepartmentName',
                    title: '收信单位'
                }, {
                    field: 'sendDepartmentName',
                    title: '发信单位'
                }, {
                    field: 'secretLevel',
                    title: '秘密等级',
                    formatter: function (value) {
                        var secretLevelText;
                        switch (value) {
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
                }, {
                    field: 'urgencyLevel',
                    title: '缓急程度',
                    formatter: function (value) {
                        var urgencyLevelText;
                        switch (value) {
                            case 0:
                                urgencyLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                urgencyLevelText = '<label class="label label-info"> 紧急</label>';
                                break;
                            case 2:
                                urgencyLevelText = '<label class="label label-warning">特急</label>';
                                break;
                            case 3:
                                urgencyLevelText = '<label class="label label-danger"> 限时</label>';
                                break;
                            default:
                                urgencyLevelText = "未知";
                                break;
                        }
                        return urgencyLevelText;
                    }
                }, {
                    field: 'urgencyTime',
                    title: '限时时间'
                }, {
                    field: 'id',
                    title: '操作',
                    formatter: function (value) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-warning btn-xs change-jytx" data-letter-id=' + value + '>转机要通信</button>';
                        return strHtml;
                    }
                }
            ]
        });
    }

    //生成同城交换清单
    function createTcjhList(nextFunc) {
        if (tcjhListId !== undefined) {
            if (nextFunc === "writeCpuCard") {
                writeCpuCard();
            } else if (nextFunc === "printTcjhBill") {
                printTcjhBill();
            }
        } else {
            var selections = tcjhTable.bootstrapTable('getSelections');
            if (selections.length > 0) {
                var letterIds = '';
                for (var i = 0; i < selections.length; i++) {
                    letterIds += ',' + selections[i].id;
                }
                letterIds = letterIds.substring(1);
                var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
                $.ajax({
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: { letterIds: letterIds },
                    headers:
                        {
                            "BugChang-CSRF-HEADER": token //注意header要修改
                        },
                    url: "/Letter/CreateTcjhList",
                    success: function (result) {
                        if (result.success) {
                            tcjhListId = result.data;
                            if (nextFunc === "writeCpuCard") {
                                writeCpuCard();
                            } else if (nextFunc === "printTcjhBill") {
                                printTcjhBill();
                            }
                        } else {
                            window.toastr.error(result.message);
                        }
                    }
                });

            } else {
                window.toastr.error("未选中任何记录");
            }
        }

    }

    //生成直送清单
    function createZsList() {
        if (zsListId !== undefined) {
            printZsBill();
        } else {
            var selections = zsTable.bootstrapTable('getSelections');
            if (selections.length > 0) {
                var letterIds = '';
                for (var i = 0; i < selections.length; i++) {
                    letterIds += ',' + selections[i].id;
                }
                letterIds = letterIds.substring(1);
                var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
                $.ajax({
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: { letterIds: letterIds },
                    headers:
                        {
                            "BugChang-CSRF-HEADER": token //注意header要修改
                        },
                    url: "/Letter/CreateZsList",
                    success: function (result) {
                        if (result.success) {
                            zsListId = result.data;
                            printZsBill();
                        } else {
                            window.toastr.error(result.message);
                        }
                    }
                });

            } else {
                window.toastr.error("未选中任何记录");
            }
        }

    }

    //生成机要通信清单
    function createJytxList() {
        if (jytxListId !== undefined) {
            printJytxBill();
        } else {
            var selections = jytxScanTable.bootstrapTable('getData');
            if (selections.length > 0) {
                var letterIds = '';
                for (var i = 0; i < selections.length; i++) {
                    letterIds += ',' + selections[i].id;
                }
                letterIds = letterIds.substring(1);
                var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
                $.ajax({
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: { letterIds: letterIds },
                    headers:
                        {
                            "BugChang-CSRF-HEADER": token //注意header要修改
                        },
                    url: "/Letter/CreateJytxList",
                    success: function (result) {
                        if (result.success) {
                            jytxListId = result.data;
                            printJytxBill();
                        } else {
                            window.toastr.error(result.message);
                        }
                    }
                });

            } else {
                window.toastr.error("未添加任何记录");
            }
        }
    }

    //转市机
    function changeJytx(letterId) {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: null,
            headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
            url: "/Letter/Change2Jytx/" + letterId,
            success: function (result) {
                if (result.success) {
                    tcjhTable.bootstrapTable('refresh', { silent: true });
                    zsTable.bootstrapTable('refresh', { silent: true });
                    jytxTable.bootstrapTable('refresh', { silent: true });
                    jytxScanTable.bootstrapTable('refresh', { silent: true });
                } else {
                    window.toastr.error(result.message);
                }
            }
        });
    }

    //写卡
    function writeCpuCard() {
        $.get("/Letter/GetWriteCpuCardData/",
            { listId: tcjhListId },
            function (result) {
                if (result.success) {
                    $.get("/HardWare/GetCpuReadCard",
                        { deviceCode: deviceCode },
                        function (data) {
                            var row = { command: 'WriteCpuCard', port: data.value.replace("COM", ""), rate: data.baudRate, text: result.data };
                            socket.send(JSON.stringify(row));
                        });
                }
            });
    }

    //打印同城交换清单
    function printTcjhBill() {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: null,
            headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
            url: "/Letter/SortingPrintTcjh/" + tcjhListId,
            success: function (html) {
                var lodop = getLodop();
                lodop.PRINT_INIT("");
                var style = '<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse;line-height:30px}</style>';
                lodop.ADD_PRINT_TABLE("2%", "5%", "90%", "96%", style + html);
                lodop.PRINT();
            }
        });
    }

    //打印直送交换清单
    function printZsBill() {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: null,
            headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
            url: "/Letter/SortingPrintZs/" + zsListId,
            success: function (html) {
                var lodop = getLodop();
                lodop.PRINT_INIT("");
                var style = '<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse;line-height:30px}</style>';
                lodop.ADD_PRINT_TABLE("2%", "5%", "90%", "96%", style + html);
                lodop.PRINT();
            }
        });
    }

    //打印机要通信清单
    function printJytxBill() {
        var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
        $.ajax({
            type: 'POST',
            async: false,
            cache: false,
            data: null,
            headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
            url: "/Letter/SortingPrintJytx/" + jytxListId,
            success: function (html) {
                for (var i = 0; i < 1; i++) {
                    var lodop = getLodop();
                    lodop.PRINT_INIT("");
                    var style = '<style>table{border-collapse:collapse;border:none;line-height:30px}th,td{border:solid #000 1px;} table tbody tr td {font-size: 14px;height:30px} table tfoot{font-size: 14px;height:40px}</style>';
                    //不断调整500的值，直到15行一页为止
                    lodop.ADD_PRINT_TABLE("2%", "5%", "90%", 500, style + html);
                    lodop.SET_PRINT_STYLEA(0, "TableRowThickNess", 20);
                    //lodop.SET_PRINT_MODE("PRINT_PAGE_PERCENT", "84%");
                    lodop.PRINT();
                }

            }
        });
    }




    //市机排序
    function doSort(letterId) {
        var row = jytxTable.bootstrapTable('getRowByUniqueId', letterId);
        if (row !== null && row !== undefined) {
            jytxScanTable.bootstrapTable('append', row);
            jytxTable.bootstrapTable('removeByUniqueId', letterId);
        } else {
            window.toastr.error("排序失败，表格中不存在记录");
        }
    }

    //市机取消排序
    function doUnSort(letterId) {
        var row = jytxScanTable.bootstrapTable('getRowByUniqueId', letterId);
        jytxTable.bootstrapTable('append', row);
        jytxTable.bootstrapTable('scrollTo', 'bottom');
        jytxScanTable.bootstrapTable('removeByUniqueId', letterId);
    }
})();