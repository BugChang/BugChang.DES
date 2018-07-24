var LetterReceive = function () {
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
                        printBarcode(result.data);
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

    });

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
        $.get('/Letter/GetReceiveDetailSelect/' + groupId,
            function (nodes) {
                groupDetailTreeObj = $.fn.zTree.init($('#groupDetailTree'), groupDetailSetting, nodes);
            });
    }

    function printBarcode(letterId) {
        var myDate = new Date();
        var lodop = getLodop();
        lodop.PRINT_INIT("");
        lodop.SET_PRINT_MODE("PRINT_NOCOLLATE", 1);
        lodop.PRINT_INIT("");
        lodop.SET_PRINT_MODE("PRINT_NOCOLLATE", 1);
        lodop.ADD_PRINT_BARCODE("7.81mm", "5.78mm", "15.01mm", "66.99mm", "128Auto","123456789012345678901234567890123");
        lodop.SET_PRINT_STYLEA(0, "ShowBarText", 0);
        lodop.SET_PRINT_STYLEA(0, "Angle", 90);
        lodop.ADD_PRINT_LINE("12.87mm", "22.75mm", "12.87mm", "117.75mm", 0, 1);
        lodop.ADD_PRINT_LINE("19.26mm", "22.97mm", "19.26mm", "117.96mm", 0, 1);
        lodop.ADD_PRINT_LINE("68.33mm", "24.77mm", "68.33mm", "119.76mm", 0, 1);
        lodop.ADD_PRINT_TEXT("6.56mm", "22.75mm", "24.45mm", "6.01mm", "秘密");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
        lodop.ADD_PRINT_TEXT("6.56mm", "46.67mm", "74.89mm", "6.01mm", "信封号：1234567");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
        lodop.ADD_PRINT_TEXT("13.86mm", "22.86mm", "44.6mm", "6.01mm", "缓急：特急");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
        if (false) {
            lodop.ADD_PRINT_TEXT("26.73mm", "23.28mm", "43.6mm", "6.01mm", "北京市");
            lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
        }

        lodop.ADD_PRINT_TEXT("33.34mm", "23.71mm", "97.9mm", "26.16mm", "一局三处");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 28);
        lodop.SET_PRINT_STYLEA(0, "Alignment", 2);
        lodop.SET_PRINT_STYLEA(0, "LineSpacing", "-0.21mm");
        lodop.SET_PRINT_STYLEA(0, "LetterSpacing", "-0.21mm");
        lodop.ADD_PRINT_TEXT("59.61mm", "23.71mm", "98.32mm", "7.81mm", "张三（收）");
        lodop.SET_PRINT_STYLEA(0, "FontName", "黑体");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 22);
        lodop.SET_PRINT_STYLEA(0, "Alignment", 3);
        lodop.ADD_PRINT_TEXT("70.61mm", "24.72mm", "70.02mm", "6.01mm", "北京市国家安全局一局六处");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
        lodop.SET_PRINT_STYLEA(0, "Bold", 1);
        lodop.ADD_PRINT_TEXT("70.61mm", "94.72mm", "27.66mm", "6.01mm", '2018-' + parseInt(myDate.getMonth() + 1) + '-' + myDate.getDate());
        lodop.SET_PRINT_STYLEA(0, "FontSize", 12);
        lodop.ADD_PRINT_LINE("26.54mm", "23.43mm", "26.54mm", "119.23mm", 0, 1);
        lodop.ADD_PRINT_TEXT("20.57mm", "23.14mm", "44.7mm", "5.29mm", "原始编号：1111");
        lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
        if (false) {
            lodop.ADD_PRINT_TEXT("27.09mm", "68.58mm", "52.92mm", "5.29mm", "箱号:026");
            lodop.SET_PRINT_STYLEA(0, "FontSize", 16);
            lodop.SET_PRINT_STYLEA(0, "Alignment", 3);
            lodop.SET_PRINT_STYLEA(0, "Bold", 1);

        }
        lodop.ADD_PRINT_TEXT("13.12mm", "68.58mm", "52.92mm", "6.35mm", "市机码：1123" );
        lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
        lodop.ADD_PRINT_TEXT("20.53mm", "68.37mm", "53.13mm", "5.5mm", "原发单位:安全部打打杀杀多" );
        lodop.SET_PRINT_STYLEA(0, "FontSize", 14);
        //lodop.SET_PRINTER_INDEX(data.Value);
        lodop.PRINT_DESIGN();
    }
}();

