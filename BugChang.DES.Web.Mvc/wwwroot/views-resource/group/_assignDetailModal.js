(function () {
    var zTreeObj;
    var groupId = $("#DefaultValue").attr("data-group-id");
    // zTree 参数配置
    var setting = {
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId",
                rootPId: null
            }
        },
        check: {
            enable: true,
            chkboxType: { "Y": "", "N": "" }
        }
    };
    $(function () {

        initTree();

        $("#autoRelation").click(function () {
            var flag= $(this).is(":checked");
            autoRelation(flag);
        });

        $("#AssignDetailForm").submit(function (e) {
            e.preventDefault();
            $.post('/Group/AssignDetail',
                { groupId: groupId, strDetailId: getStrDetailId() },
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#AssignDetailModal").modal("hide");
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

    });

    function initTree() {
        $.get('/Group/GetGroupDetails/' + groupId,
            function (nodes) {
                zTreeObj = $.fn.zTree.init($('#ztree'), setting, nodes);
            });
    }

    //获取选中的菜单Id字符串
    function getStrDetailId() {
        var nodes = zTreeObj.getCheckedNodes(true);
        var strDetailId = '';
        $.each(nodes, function (i, node) {
            strDetailId += ',' + node.id;
        });
        strDetailId = strDetailId.substring(1);
        return strDetailId;
    }

    function autoRelation(flag) {
        if (flag) {
            setting.check.chkboxType = { "Y": "ps", "N": "ps" };
        } else {
            setting.check.chkboxType = { "Y": "", "N": "" };
        }

        initTree();
    }
})();