var LetterSend = function () {
    var table;
    var socket;
    var deviceCode;
    var groupTreeObj;
    var groupDetailTreeObj;
    var currentId;
    // zTree 参数配置
    var groupSetting = {
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId",
                rootPId: null
            }
        },
        callback: {
            beforeClick: groupTreeBeforeClick
        }
    };
    var groupDetailSetting = {
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId",
                rootPId: null
            }
        },
        view: {
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom
        }
    };
    $(function () {

        initSocket();

        initGroupTree();

        initTable();

        $("#UrgencyLevel").change(function () {
            if ($(this).val() === "3") {
                $("#divUrgencyTime").removeClass("hide");
            } else {
                $("#UrgencyTime").val("");
                $("#divUrgencyTime").addClass("hide");
            }

        });

        $('#UrgencyTime').datetimepicker({
            format: 'yyyy-mm-dd hh:ii',
            language: 'zh-CN'
        });

        $('#groupDetailTree').delegate('.department-select',
            'click',
            function () {
                var departmentId = $(this).attr('data-department-id');
                var departmentName = $(this).attr('data-department-name');
                $("#btnReceiveDepartment").text("已选择：" + departmentName);
                $("#ReceiveDepartmentId").val(departmentId);
                $('#ReceiveSelectModal').modal('hide');
            });

        $("#btnPrintBarcode").click(function () {
            var data = $("#LetterSendForm").serialize();
            $.post('/Letter/Send',
                data,
                function (result) {
                    if (result.success) {
                        window.toastr.success('操作成功,正在打印条码...');
                        refresh();
                        printBarcode(result.data);
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        $('table').delegate('.print-barcode',
            'click',
            function () {
                var letterId = $(this).attr('data-letter-id');
                printBarcode(letterId);
            });

    });

    //初始化table
    function initTable() {
        table = $('#table').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: '/Letter/GetTodaySendLetters'
            },
            stateSave: true,
            columns: [
                {
                    data: null,
                    title: '操作'
                },
                {
                    data: 'letterNo',
                    title: '信封编号'
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
                    targets: 5,
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
                    targets: 6,
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
                    targets: 0,
                    render: function (data, type, row) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-primary btn-xs print-barcode" data-letter-id=' + row.id + '>补打条码</button>&nbsp;';
                        strHtml += '<button class="btn btn-danger btn-xs delete-letter" data-letter-id=' + row.id + ' data-letter-no=' + row.barcodeNo.substring(15, 22) + '>删除</button>';
                        return strHtml;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

    function groupTreeBeforeClick(treeId, treeNode) {
        initGroupDetailTree(treeNode.id);
    }

    function addHoverDom(treeId, treeNode) {
        if (currentId !== treeNode.id) {
            var aObj = $("#" + treeNode.tId + "_a");
            var editStr = '<button type="button" class="btn btn-link department-select" data-department-id="' + treeNode.id + '" data-department-name="' + treeNode.name + '">确定</button>';
            aObj.append(editStr);
            currentId = treeNode.id;
        }

    }

    function removeHoverDom() {
        $(".department-select").unbind().remove();
        currentId = null;
    }

    function initGroupTree() {
        $.get('/Letter/GetSendGroupSelect/',
            function (nodes) {
                groupTreeObj = $.fn.zTree.init($('#groupTree'), groupSetting, nodes);
            });
    }

    function initGroupDetailTree(groupId) {
        $.get('/Letter/GetDetailSelect/' + groupId,
            function (nodes) {
                groupDetailTreeObj = $.fn.zTree.init($('#groupDetailTree'), groupDetailSetting, nodes);
            });
    }

    function printBarcode(letterId) {
        $.get("/HardWare/GetBarcodePrint80130",
            { deviceCode: deviceCode },
            function (hard) {
                $.get("/Letter/GetSendBarcode/" + letterId, function (data) {
                    var lodop = getLodop();
                    lodop.PRINT_INIT("");
                    lodop.SET_PRINT_PAGESIZE(2, 800, 1300, "80*130");
                    lodop.SET_PRINT_MODE("PRINT_NOCOLLATE", 1);
                    //条形码
                    lodop.ADD_PRINT_BARCODE("4.42mm", "5.78mm", "15.01mm", "66.99mm", "128Auto", data.barcodeNo);
                    lodop.SET_PRINT_STYLEA(0, "ShowBarText", 0);
                    lodop.SET_PRINT_STYLEA(0, "Angle", 90);
                    lodop.ADD_PRINT_LINE("9.19mm", "22.54mm", "9.19mm", "120.29mm", 0, 1);
                    lodop.ADD_PRINT_LINE("15.92mm", "22.54mm", "15.92mm", "120.42mm", 0, 1);
                    lodop.ADD_PRINT_LINE("68.3mm", "23.28mm", "68.3mm", "120.61mm", 0, 1);
                    //密级
                    lodop.ADD_PRINT_TEXT("3.18mm", "22.12mm", "24.45mm", "7.07mm", data.secretLevel);
                    LODOP.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
                    //信封号
                    lodop.ADD_PRINT_TEXT("3.18mm", "47.1mm", "74.46mm", "7.07mm", "信封号：" + data.letterNo);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
                    //紧急程度
                    lodop.ADD_PRINT_TEXT("10.05mm", "22.01mm", "99.65mm", "7.07mm", "缓急：" + data.urgencyLevel);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
                    //收件单位
                    lodop.ADD_PRINT_TEXT("35.94mm", "22.23mm", "100.01mm", "23.79mm", data.receiveDepartmentName);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "华文中宋");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 26);
                    lodop.SET_PRINT_STYLEA(0, "Alignment", 2);
                    lodop.SET_PRINT_STYLEA(0, "LineSpacing", "-0.21mm");
                    lodop.SET_PRINT_STYLEA(0, "LetterSpacing", "-0.21mm");
                    //收件人
                    lodop.ADD_PRINT_TEXT("59.61mm", "22.44mm", "99.59mm", "7.81mm", (data.receiver === null ? "" : data.receiver) + "（收）");
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 20);
                    lodop.SET_PRINT_STYLEA(0, "Alignment", 3);
                    //发件单位
                    lodop.ADD_PRINT_TEXT("69.55mm", "23.03mm", "70.02mm", "6.01mm",data.sendDepartmentName);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
                    lodop.SET_PRINT_STYLEA(0, "Bold", 1);
                    //打印日期
                    lodop.ADD_PRINT_TEXT("69.34mm", "97.68mm", "24.49mm", "6.01mm", data.printDate);
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
                    var ysbh = data.oldBarcodeNo == null ? "" : data.oldBarcodeNo;
                    var sjm = data.shiJiCode == null ? "" : data.shiJiCode;
                    var yfdw = data.oldDepartmentName == null ? "" : data.oldDepartmentName;
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
                    lodop.ADD_PRINT_LINE("22.48mm", "22.58mm", "22.48mm", "120.29mm", 0, 1);
                    //原始编号
                    lodop.ADD_PRINT_TEXT("16.76mm", "22.08mm", "99.76mm", "6.35mm", "原始编号：" + ysbh + "  " + sjm + "  " + yfdw);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
                    //箱号
                    lodop.ADD_PRINT_TEXT("23.24mm", "85.33mm", "35.1mm", "9.1mm", data.boxNo);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 18);
                    //收件单位所在地址
                    lodop.ADD_PRINT_TEXT("23.24mm", "22.28mm", "60.1mm", "9.1mm", data.address);
                    lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
                    lodop.SET_PRINT_STYLEA(0, "FontSize", 18);
                    lodop.SET_PRINTER_INDEX(hard.value);
                    lodop.PRINT_DESIGN();
                });
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


    //刷新页面
    function refresh() {
        //刷新表格
        table.draw(false);
    }
}();

