(function () {
    var userId = $('#userId').val();
    var userRoleTable;
    $(function () {
        initRoleList();
        initUserRoleTable();


        $('#btnAddUserRole').click(function () {
            var roleId = $('#roleId').val();
            $.post('/User/AddUserRole',
                { userId: userId, roleId: roleId },
                function (result) {
                    if (result.success) {
                        window.toastr.success('操作成功');
                        userRoleTable.ajax.reload();
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        $('table').delegate('.delete-user-role',
            'click',
            function () {
                var roleId = $(this).attr('data-role-id');
                var roleName = $(this).attr('data-role-name');
                deleteUserRole(roleId, roleName);
            });
    });


    function initUserRoleTable() {
        userRoleTable = $('#userRoleTable').DataTable({
            ordering: false,
            paging: false,
            searching: false,
            info: false,
            processing: true,
            autoWith: true,
            ajax: {
                url: '/User/GetUserRoles/' + userId
            },
            columns: [
                {
                    data: 'id',
                    title: '主键'

                },
                {
                    data: 'name',
                    title: '名称'
                },
                {
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 2,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation('Role.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-user-role" data-role-id=' + row.id + ' data-role-name=' + row.name + '>删除</button>';
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


    //初始化角色列表
    function initRoleList() {
        $.get("/User/GetRolesForSelect",
            function (data) {
                $("#roleId").select2({
                    data: data
                });
            });
    }

    function deleteUserRole(roleId, roleName) {
        window.swal({
            title: '确定删除' + roleName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/User/DeleteUserRole/', {
                    userId: userId,
                    roleId: roleId
                }, function (result) {
                    if (result.success) {
                        window.swal('操作成功', roleName + '已被删除!', 'success');
                        userRoleTable.ajax.reload();
                    } else {
                        window.swal('操作失败', result.message, 'error');
                    }
                });
            }
        });
    }
})();