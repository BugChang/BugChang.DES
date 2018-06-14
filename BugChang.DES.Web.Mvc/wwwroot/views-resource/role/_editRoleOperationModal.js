(function () {
    var zTreeObj;
    var roleId = $('#menuTree').attr('data-role-id');
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
        callback: {
            beforeClick: zTreeBeforeClick
        }
    };
    $(function () {
        //初始化zTree
        initTree();

        //监听表单提交事件
        $("#RoleOperationEditForm").submit(function (e) {
            e.preventDefault();
            $.post('/Role/EditRoleOperation',
                { roleId: roleId, strOperationId: getStrOperationId() },
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#RoleOperationEditModal").modal("hide");
                        //刷新父页面
                        RoleIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        //iCheck for checkbox and radio inputs
        $('input[type="checkbox"], input[type="radio"]').iCheck({
            checkboxClass: 'icheckbox_minimal-blue',
            radioClass: 'iradio_minimal-blue'
        });
    });

    //初始化菜单树
    function initTree() {
        $.get('/Role/GetTreeForRoleOperation/' + roleId,
            function (nodes) {
                zTreeObj = $.fn.zTree.init($('#menuTree'), setting, nodes);
            });
    }

    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode) {
        $.post('/Role/GetOperationsByUrl',
            { url: treeNode.customData },
            function(result) {
                alert(result);
            });
    }

    //获取选中的菜单Id字符串
    function getStrOperationId() {
        var nodes = zTreeObj.getCheckedNodes(true);
        var strOperationId = '';
        $.each(nodes, function (i, node) {
            strOperationId += ',' + node.id;
        });
        strOperationId = strOperationId.substring(1);
        return strOperationId;
    }
})();