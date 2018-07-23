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
}();

