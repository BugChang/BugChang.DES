(function () {
    var table;
    var socket;
    var deviceCode;
    $(function () {
        initTable();

        initSocket();

        $('table').delegate('.view-detail',
            'click',
            function () {
                var listId = $(this).attr('data-list-id');
                viewDtail(listId);
            });

        $('table').delegate('.print-bill-tcjh',
            'click',
            function () {
                var listId = $(this).attr('data-list-id');
                printBillTcjh(listId);
            });

        $('table').delegate('.print-bill-zs',
            'click',
            function () {
                var listId = $(this).attr('data-list-id');
                printBillZs(listId);
            });

        $('table').delegate('.print-bill-jytx',
            'click',
            function () {
                var listId = $(this).attr('data-list-id');
                printBillJytx(listId);
            });

        $('table').delegate('.write-card',
            'click',
            function () {
                var listId = $(this).attr('data-list-id');
                writeCard(listId);
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
            } else if (obj.Method === "WriteCpuCard") {
                if (obj.Data) {
                    window.toastr.success("写卡成功");
                } else {
                    window.toastr.error("写卡失败");
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

    //初始化table
    function initTable() {
        table = $('#table').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: '/Letter/GetSortingLists'
            },
            stateSave: true,
            columns: [
                {
                    data: 'id',
                    title: '主键'

                },
                {
                    data: 'channel',
                    title: '渠道'
                },
                {
                    data: 'listNo',
                    title: '清单号'
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
                    targets: 5,
                    render: function (data, type, row) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-primary btn-xs view-detail" data-list-id=' + row.id + '>明细</button>&nbsp;';
                        if (row.channel === "同城交换") {
                            strHtml += '<button class="btn btn-info btn-xs print-bill-tcjh" data-list-id=' + row.id + '>打印</button>&nbsp;';
                            strHtml += '<button class="btn btn-success btn-xs write-card" data-list-id=' + row.id + '>写卡</button>&nbsp;';
                        }
                        if (row.channel === "机要通信") {
                            strHtml += '<button class="btn btn-info btn-xs print-bill-jytx" data-list-id=' + row.id + '>打印</button>&nbsp;';
                        }
                        if (row.channel === "直送") {
                            strHtml += '<button class="btn btn-info btn-xs print-bill-zs" data-list-id=' + row.id + '>打印</button>&nbsp;';
                        }
                        return strHtml;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

    function printBillTcjh(listId) {
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
            url: "/Letter/SortingPrintTcjh/" + listId,
            success: function (html) {
                var lodop = getLodop();
                lodop.PRINT_INIT("");
                var style = '<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse;line-height:30px}</style>';
                lodop.ADD_PRINT_TABLE("2%", "5%", "90%", "96%", style + html);
                lodop.PRINT();
            }
        });
    }

    function printBillJytx(listId) {
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
            url: "/Letter/SortingPrintJytx/" + listId,
            success: function (html) {
                var lodop = getLodop();
                lodop.PRINT_INIT("");
                var style = '<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse;line-height:30px}</style>';
                lodop.ADD_PRINT_TABLE("2%", "5%", "90%", "96%", style + html);
                lodop.PRINT();
            }
        });
    }

    function printBillZs(listId) {
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
            url: "/Letter/SortingPrintZs/" + listId,
            success: function (html) {
                var lodop = getLodop();
                lodop.PRINT_INIT("");
                var style = '<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse;line-height:30px}</style>';
                lodop.ADD_PRINT_TABLE("2%", "5%", "90%", "96%", style + html);
                lodop.PRINT();
            }
        });
    }

    function writeCard(listId) {
        $.get("/Letter/GetWriteCpuCardData/",
            { listId: listId },
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

    function viewDtail(listId) {
        $('#DetailViewModal .modal-content').load('/Letter/SortingListDetail/' + listId);
        $('#DetailViewModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }
})();