(function () {
    var zTreeObj;
    var roleId = $('#roleOperationTree').attr('data-role-id');
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

        //操作选项状态更改时
        $('#operationList').delegate('input', 'ifChanged', function () {
            var checked = $(this).prop('checked');
            var operationCode = $(this).val();
            if (checked) {
                addRoleOperation(operationCode);
            } else {
                deleteRoleOperation(operationCode);
            }
        });
    });

    //初始化菜单树
    function initTree() {
        $.get('/Role/GetTreeForRoleOperation/' + roleId,
            function (nodes) {
                zTreeObj = $.fn.zTree.init($('#roleOperationTree'), setting, nodes);
            });
    }

    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode) {
        $.post('/Role/GetOperationsByUrl',
            { url: treeNode.customData, roleId: roleId },
            function (result) {
                $('#operationList').empty();
                $.each(result, function (idx, obj) {
                    var html = '<label class="col-md-4"><input type = "checkbox"';
                    if (obj.checked) {
                        html += ' checked = ' + obj.checked;
                    }
                    html += ' value="' + obj.operationCode + '">&nbsp;' + obj.operationName + '</label >';
                    $('#operationList').append(html);
                });
                initCheckStyle();
            });
    }

    //初始化单选、复选框样式
    function initCheckStyle() {
        //iCheck for checkbox and radio inputs
        $('input[type="checkbox"], input[type="radio"]').iCheck({
            checkboxClass: 'icheckbox_minimal-blue',
            radioClass: 'iradio_minimal-blue'
        });
    }

    //新增角色和操作关联
    function addRoleOperation(operationCode) {
        $.post('/Role/AddRoleOperation', { roleId: roleId, operationCode: operationCode }, function(result) {
            if (result.success) {
                window.toastr.success('操作成功');
            } else {
                window.toastr.error(result.message);
            }
        });
    }

    //删除角色和操作关联
    function deleteRoleOperation(operationCode) {
        $.post('/Role/DeleteRoleOperation', { roleId: roleId, operationCode: operationCode }, function (result) {
            if (result.success) {
                window.toastr.success('操作成功');
            } else {
                window.toastr.error(result.message);
            }
        });
    }

})();