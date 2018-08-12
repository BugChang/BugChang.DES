﻿(function () {
    var macAddress;
    var socket;
    var tcjhTable = $("#tcjhTable");
    var jytxTable = $("#jytxTable");
    var zsTable = $("#zsTable");
    var jytxScanTable = $("#jytxScanTable");
    var tcjhListId;
    $(function () {

        initSocket();

        initTcjhTable();

        $("#btnTcjh").click(function () {

        });

        $("#btnJytx").click(function () {
            initJytxTable();
            intJytxScanTable();
        });

        $("#btnZs").click(function () {
            initZsRable();
        });

        $('tab-content').delegate('.change-jytx',
            'click',
            function () {
                var letterId = $(this).attr('data-letter-id');
                changeJytx(letterId);
            });


        $("#btnPrintTcjhBill").click(function () {
            createTcjhList();
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
        socket.onerror = function () {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }

    //打开扫描枪端口
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
            queryParams: function (params) {
                return {
                    search: params.search,
                    limit: params.limit,
                    offset: params.offset
                }
            },
            columns: [
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
    function createTcjhList() {
        var selections = tcjhTable.bootstrapTable('getSelections');
        if (selections.length > 0) {
            var letterIds = '';
            for (var i = 0; i < selections.length; i++) {
                letterIds += ',' + selections[i].id;
            }
            letterIds = letterIds.substring(1);
            $.post("/Letter/CreateTcjhList",
                { letterIds: letterIds },
                function (result) {
                    if (result.success) {
                        tcjhListId = result.data;
                    } else {
                        window.toastr.error(result.message);
                    }
                });

        } else {
            window.toastr.error("未选中任何记录");
        }
    }

    //转市机
    function changeJytx(letterId) {
        $.post("/Letter/Change2Jytx/" + letterId,
            function (result) {
                if (result.success) {
                    tcjhTable.bootstrapTable('refresh', { silent: true });
                    zsTable.bootstrapTable('refresh', { silent: true });
                    jytxTable.bootstrapTable('refresh', { silent: true });
                    jytxScanTable.bootstrapTable('refresh', { silent: true });
                } else {
                    window.toastr.error(result.message);
                }
            });
    }

})();