var LetterSend = function () {
    var table;
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
        $.get("/Letter/GetSendBarcode/" + letterId, function (data) {
            var lodop = getLodop();
            lodop.PRINT_INIT("");
            lodop.SET_PRINT_MODE("PRINT_NOCOLLATE", 1);
            lodop.ADD_PRINT_BARCODE("5mm", "6.01mm", "17.99mm", "70.01mm", "128Auto", data.barcodeNo);
            lodop.SET_PRINT_STYLEA(0, "ShowBarText", 0);
            lodop.SET_PRINT_STYLEA(0, "Angle", 90);
            lodop.ADD_PRINT_LINE("12.99mm", "30mm", "13.2mm", "130.02mm", 0, 1);
            lodop.ADD_PRINT_TEXT("5mm", "30mm", "20mm", "7.99mm", data.secretLevel);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
            lodop.ADD_PRINT_TEXT("5mm", "60.01mm", "45.01mm", "7.99mm", "信封号：" + data.letterNo);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_LINE("25.27mm", "30mm", "25mm", "130.02mm", 0, 1);
            lodop.ADD_PRINT_TEXT("17.99mm", "30mm", "30mm", "6.01mm", "缓急：" + data.urgencyLevel);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_TEXT("17.99mm", "63.95mm", "30mm", "6.01mm", data.urgencyTime);
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_TEXT("17.99mm", "108mm", "15mm", "6.01mm", "");
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
            lodop.ADD_PRINT_TEXT("26.99mm", "30mm", "60.01mm", "7.99mm", data.address);
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 18);
            lodop.ADD_PRINT_TEXT("26.99mm", "90.01mm", "35mm", "7.99mm", "箱号：" + data.boxNo);
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 18);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 2);
            lodop.ADD_PRINT_TEXT("37.99mm", "30mm", "100.01mm", "11.96mm", "北京市国家安全局");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 28);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 2);
            lodop.ADD_PRINT_LINE("65.01mm", "30mm", "65.22mm", "130.02mm", 0, 1);
            lodop.ADD_PRINT_TEXT("54.77mm", "69.69mm", "60.01mm", "9.31mm", data.receiver + "（收）");
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 22);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 3);
            lodop.ADD_PRINT_TEXT("68mm", "30mm", "48.1mm", "4.68mm", data.sendDepartmentName);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
            lodop.SET_PRINT_STYLEA(0, "Bold", 1);
            lodop.ADD_PRINT_TEXT("68mm", "81.86mm", "44.13mm", "4.68mm", data.printDate);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 3);
            lodop.PRINT_DESIGN();
        });
    }


    //刷新页面
    function refresh() {
        //刷新表格
        table.ajax.reload();
    }
}();

