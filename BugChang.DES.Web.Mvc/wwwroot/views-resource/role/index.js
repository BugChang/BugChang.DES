var RoleIndex = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('Role');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //新增角色表单提交
        $('#RoleCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            try {
                $.post('/Role/Edit',
                    data,
                    function (result) {
                        if (result.success) {
                            resetForm();
                            //关闭模态
                            $('#RoleCreateModal').modal('hide');
                            //刷新页面
                            refresh();
                            window.toastr.success('操作成功');
                        } else {
                            window.toastr.error(result.message);
                        }
                    });
            } catch (e) {
                console.log(e);
            }

        });

        $('table').delegate('.edit-role',
            'click',
            function () {
                var roleId = $(this).attr('data-role-id');
                editRole(roleId);
            });

        $('table').delegate('.edit-menu',
            'click',
            function () {
                var roleId = $(this).attr('data-role-id');
                editMenu(roleId);
            });

        $('table').delegate('.edit-operation',
            'click',
            function () {
                var roleId = $(this).attr('data-role-id');
                editOperation(roleId);
            });

        $('table').delegate('.delete-role',
            'click',
            function () {
                var roleId = $(this).attr('data-role-id');
                var roleName = $(this).attr('data-role-name');
                deleteRole(roleId, roleName);
            });

        $('#btnRefresh').click(function () {
            reload();
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
                url: '/Role/GetListForTable'
            },
            stateSave: true,
            columns: [
                {
                    data: 'id',
                    title: '主键'

                },
                {
                    data: 'name',
                    title: '角色名称'
                },
                {
                    data: 'description',
                    title: '角色描述'
                },
                {
                    data: 'createUserName',
                    title: '创建人'
                },
                {
                    data: 'createTime',
                    title: '创建时间'
                },
                {
                    data: 'updateUserName',
                    title: '最后更改人'
                },
                {
                    data: 'updateTime',
                    title: '最后更改时间'
                },
                {
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 7,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation('Role.AssignmentsMenus')) {
                            strHtml += '<button class="btn btn-primary btn-xs edit-menu" data-role-id=' + row.id + '>菜单分配</button>&nbsp;';
                        }
                        if (Common.hasOperation('Role.AssignmentsOperations')) {
                            strHtml += '<button class="btn btn-primary btn-xs edit-operation" data-role-id=' + row.id + '>操作权限</button>&nbsp;';
                        }
                        if (Common.hasOperation('Role.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-role" data-role-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('Role.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-role" data-role-id=' + row.id + ' data-role-name=' + row.name + '>删除</button>';
                        }

                        return strHtml;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

    //编辑角色信息
    function editRole(id) {
        $('#RoleEditModal .modal-content').load('/Role/EditRoleModal/' + id);
        $('#RoleEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除角色
    function deleteRole(roleId, roleName) {
        window.swal({
            title: '确定删除' + roleName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
                $.ajax({
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: null,
                    headers:
                    {
                        "BugChang-CSRF-HEADER": token //注意header要修改
                    },
                    url: "/Role/Delete/" + roleId,
                    success: function (result) {
                        if (result.success) {
                            window.swal('操作成功', roleName + '已被删除!', 'success');
                            refresh();
                        } else {
                            window.swal('操作失败', result.message, 'error');
                        }
                    }
                });
            }
        });
    }

    //菜单分配
    function editMenu(roleId) {
        $('#RoleMenuEditModal .modal-content').load('/Role/EditRoleMenuModal/' + roleId);
        $('#RoleMenuEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //操作权限分配
    function editOperation(roleId) {
        $('#RoleOperationEditModal .modal-content').load('/Role/EditRoleOperationModal/' + roleId);
        $('#RoleOperationEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //清空表单
    function resetForm() {
        $('#RoleCreateForm')[0].reset();
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.draw(false);
    }

    //重新加载页面
    function reload() {
        window.location.reload();
    }

    //初始化页面元素
    function initPageElement() {
        if (!Common.hasOperation('Role.Create')) {
            $('#btnAddRole').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();

