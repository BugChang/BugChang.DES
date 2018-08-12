var LetterReceive = function () {
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
            var data = $("#LetterReceiveForm").serialize();
            $.post('/Letter/Receive',
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
                url: '/Letter/GetTodayReceiveLetters'
            },
            stateSave: true,
            columns: [
                {
                    data: null,
                    title: '操作'
                },
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
                    targets: 1,
                    render: function (data, type, row) {

                        return row.barcodeNo.substring(15, 22);
                    }
                },
                {
                    targets: 7,
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
                    targets: 8,
                    render: function (data, type, row) {
                        var urgencyLevelText;
                        switch (row.urgencyLevel) {

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
        $.get('/Letter/GetReceiveGroupSelect/',
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
        $.get("/Letter/GetReceiveBarcode/" + letterId, function (data) {
            var lodop = getLodop();
            lodop.PRINT_INIT("");
            lodop.SET_PRINT_MODE("PRINT_NOCOLLATE", 1);
            lodop.PRINT_INIT("");
            lodop.SET_PRINT_MODE("PRINT_NOCOLLATE", 1);
            lodop.ADD_PRINT_BARCODE("7.11mm", "5.78mm", "15.01mm", "66.99mm", "128Auto", data.barcodeNo);
            lodop.SET_PRINT_STYLEA(0, "ShowBarText", 0);
            lodop.SET_PRINT_STYLEA(0, "Angle", 90);
            lodop.ADD_PRINT_LINE("12.87mm", "22.75mm", "12.87mm", "117.75mm", 0, 1);
            lodop.ADD_PRINT_LINE("19.26mm", "22.97mm", "19.26mm", "117.96mm", 0, 1);
            lodop.ADD_PRINT_LINE("68.33mm", "24.77mm", "68.33mm", "119.76mm", 0, 1);
            lodop.ADD_PRINT_TEXT("6.56mm", "22.75mm", "24.45mm", "6.01mm", data.secretLevel);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
            lodop.ADD_PRINT_TEXT("6.56mm", "46.67mm", "74.89mm", "6.01mm", "信封号：" + data.barcodeNo.substring(15, 22));
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_TEXT("13.86mm", "22.86mm", "44.6mm", "6.01mm", "缓急：" + data.urgencyLevel);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_TEXT("33.34mm", "23.71mm", "97.9mm", "26.16mm", data.receiveDepartmentName);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 28);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 2);
            lodop.SET_PRINT_STYLEA(0, "LineSpacing", "-0.21mm");
            lodop.SET_PRINT_STYLEA(0, "LetterSpacing", "-0.21mm");
            lodop.ADD_PRINT_TEXT("59.61mm", "23.71mm", "98.32mm", "7.81mm", data.receiver + "（收）");
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 22);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 3);
            lodop.ADD_PRINT_TEXT("70.61mm", "24.72mm", "70.02mm", "6.01mm", data.sendDepartmentName);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
            lodop.SET_PRINT_STYLEA(0, "Bold", 1);
            lodop.ADD_PRINT_TEXT("70.61mm", "94.72mm", "27.66mm", "6.01mm", data.printDate);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
            lodop.ADD_PRINT_LINE("26.54mm", "23.43mm", "26.54mm", "119.23mm", 0, 1);
            lodop.ADD_PRINT_TEXT("20.57mm", "23.14mm", "44.7mm", "5.29mm", "原始编号：" + data.oldBarcodeNo);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_TEXT("13.12mm", "68.58mm", "52.92mm", "6.35mm", "市机码：" + data.customData);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            lodop.ADD_PRINT_TEXT("20.53mm", "68.37mm", "53.13mm", "5.5mm", "原发单位：" + data.oldSendDepartmentName);
            lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
            //lodop.SET_PRINTER_INDEX(data.Value);
            lodop.PRINT_DESIGN();
        });
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.ajax.reload();
    }
}();

