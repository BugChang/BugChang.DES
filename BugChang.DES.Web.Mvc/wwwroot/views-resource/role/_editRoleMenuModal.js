﻿(function () {
    var zTreeObj;
    var roleId = $('#roleMenuTree').attr('data-role-id');
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
            enable: true
        }
    };
    $(function () {
        //初始化zTree
        initTree();

        //监听表单提交事件
        $("#RoleMenuEditForm").submit(function (e) {
            e.preventDefault();
            var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
            $.ajax({
                type: 'POST',
                async: false,
                cache: false,
                data: { roleId: roleId, strMenuId: getStrMenuId() },
                headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
                url: "/Role/EditRoleMenu",
                success: function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#RoleMenuEditModal").modal("hide");
                        //刷新父页面
                        RoleIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                }
            });
        });
    });

    //初始化菜单树
    function initTree() {
        $.get('/Role/GetTreeForRoleMenu/' + roleId,
            function (nodes) {
                zTreeObj = $.fn.zTree.init($('#roleMenuTree'), setting, nodes);
            });
    }

    //获取选中的菜单Id字符串
    function getStrMenuId() {
        var nodes = zTreeObj.getCheckedNodes(true);
        var strMenuId = '';
        $.each(nodes, function (i, node) {
            strMenuId += ',' + node.id;
        });
        strMenuId = strMenuId.substring(1);
        return strMenuId;
    }
})();