var UserIndex = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('User');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //初始化select2选择框
        initSelect();

        //新增菜单表单提交
        $('#UserCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/User/Create',
                data,
                function (result) {
                    if (result.success) {
                        resetForm();
                        //关闭模态
                        $('#UserCreateModal').modal('hide');
                        //刷新页面
                        refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        $('table').delegate('.edit-user',
            'click',
            function () {
                var userId = $(this).attr('data-user-id');
                editUser(userId);
            });

        $('table').delegate('.delete-user',
            'click',
            function () {
                var userId = $(this).attr('data-user-id');
                var userName = $(this).attr('data-user-name');
                deleteUser(userId, userName);
            });

        $('table').delegate('.edit-user-role',
            'click',
            function () {
                var userId = $(this).attr('data-user-id');
                editUserRole(userId);
            });

        $('table').delegate('.enabled-user',
            'click',
            function () {
                var userId = $(this).attr('data-user-id');
                changeUserEnabled(userId);
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
            select: true,
            ajax: {
                url: '/User/GetListForTable'
            },
            stateSave: true,
            columns: [
                {
                    data: 'id',
                    title: '主键'

                },
                {
                    data: 'userName',
                    title: '用户名'
                },
                {
                    data: 'displayName',
                    title: '姓名'
                },
                {
                    data: 'departmentName',
                    title: '所属机构'
                },
                {
                    data: 'locked',
                    title: '锁定状态'
                },
                {
                    data: 'enabled',
                    title: '启用状态'
                },
                {
                    data: 'phone',
                    title: '移动电话'
                },
                {
                    data: 'tel',
                    title: '固定电话'
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
                    targets: 4,
                    render: function (data, type, row) {
                        var strHtml;
                        if (row.locked) {
                            strHtml = '<label class="label label-danger"><i class="fa fa-lock"></i> 已锁定</label>&nbsp;';
                        } else {
                            strHtml = '<label class="label label-success"><i class="fa fa-unlock"></i> 未锁定</label>&nbsp;';
                        }

                        return strHtml;
                    }
                },
                {
                    targets: 5,
                    render: function (data, type, row) {
                        var strHtml;
                        if (Common.hasOperation('User.Enabled')) {
                            if (row.enabled) {
                                strHtml = '<button class="btn btn-danger btn-xs enabled-user" data-user-id=' + row.id + '>禁用</button>&nbsp;';
                            } else {
                                strHtml = '<button class="btn btn-success btn-xs enabled-user" data-user-id=' + row.id + '>启用</button>&nbsp;';
                            }
                        } else {
                            if (row.enabled) {
                                strHtml = '<label class="label label-success">已启用</label>';
                            } else {
                                strHtml = '<label class="label label-danger">已停用</label>';
                            }
                        }

                        return strHtml;
                    }
                },
                {
                    targets: 8,
                    render: function (data, type, row) {
                        var strHtml;
                        if (!row.createUserName) {
                            strHtml = '系统';
                        } else {
                            strHtml = row.createUserName;
                        }

                        return strHtml;
                    }
                },
                {
                    targets: 12,
                    render: function (data, type, row) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-primary btn-xs edit-user-role" data-user-id=' + row.id + '>分配角色</button>&nbsp;';
                        if (Common.hasOperation('User.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-user" data-user-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('User.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-user" data-user-id=' + row.id + ' data-user-name=' + row.displayName + '>删除</button>';
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

    //初始化上级菜单
    function initSelect() {
        $.get('/User/GetListForSelect',
            function (data) {
                $('.select2').select2({
                    data: data,
                    placeholder: '请选择上级菜单',
                    allowClear: true
                });
            });
    }

    //修改用户
    function editUser(id) {
        $('#UserEditModal .modal-content').load('/User/EditUserModal/' + id);
        $('#UserEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除用户
    function deleteUser(userId, userName) {
        window.swal({
            title: '确定删除' + userName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/User/Delete/' + userId,
                    function (result) {
                        if (result.success) {
                            window.swal('操作成功', userName + '已被删除!', 'success');
                            refresh();
                        } else {
                            window.swal('操作失败', result.message, 'error');
                        }
                    });
            }
        });
    }

    //修改用户
    function editUserRole(userId) {
        $('#UserRoleEditModal .modal-content').load('/User/EditUserRoleModal/' + userId);
        $('#UserRoleEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //清空表单
    function resetForm() {
        $('#UserCreateForm')[0].reset();
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.ajax.reload();
    }

    //重新加载页面
    function reload() {
        window.location.reload();
    }

    //初始化页面元素
    function initPageElement() {
        if (!Common.hasOperation('User.Create')) {
            $('#btnAddUser').hide();
        }
    }

    //切换用户启用状态
    function changeUserEnabled(id) {
        $.post("/User/ChangeUserEnabled/" + id,
            function (result) {
                if (result.success) {
                    window.toastr.success('操作成功');
                    refresh();
                } else {
                    window.toastr.error(result.message);
                }
            });
    }

    //向外暴露方法
    return { refresh: refresh };
}();

