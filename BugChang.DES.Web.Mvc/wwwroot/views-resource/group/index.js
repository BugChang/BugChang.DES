var GroupIndex = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('Group');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //初始化条码类型
        initTypes();

        //新增
        $('#GroupCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            try {
                $.post('/Group/Create',
                    data,
                    function (result) {
                        if (result.success) {
                            resetForm();
                            //关闭模态
                            $('#GroupCreateModal').modal('hide');
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

        $('table').delegate('.edit-group',
            'click',
            function () {
                var groupId = $(this).attr('data-group-id');
                editGroup(groupId);
            });

        $('table').delegate('.delete-group',
            'click',
            function () {
                var groupId = $(this).attr('data-group-id');
                var groupName = $(this).attr('data-group-name');
                deleteGroup(groupId, groupName);
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
                url: '/Group/GetGroups'
            },
            stateSave: true,
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
                    data: 'type',
                    title: '条码类型'
                },
                {
                    data: 'description',
                    title: '描述'
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
                    targets: 3,
                    render: function (data, type, row) {
                        var strHtml = '<span title="' + row.description + '">';
                        if (row.description.length > 20) {
                            strHtml += row.description.substring(0, 20) + '...';
                        } else {
                            strHtml += row.description;
                        }
                        strHtml += '</span>';
                        return strHtml;
                    }
                },
                {
                    targets: 8,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation('Group.AssignDetail')) {
                            strHtml += '<button class="btn btn-primary btn-xs assign-detail" data-group-id=' + row.id + '>分配单位</button>&nbsp;';
                        }
                        if (Common.hasOperation('Group.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-group" data-group-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('Group.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-group" data-group-id=' + row.id + ' data-group-name=' + row.name + '>删除</button>';
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

    //初始化分组类型
    function initTypes() {
        $.get('/Group/GetGroupTypes',
            function (data) {
                $('.type-select').select2({
                    data: data,
                    allowClear: false
                });
            });
    }


    //编辑
    function editGroup(id) {
        $('#GroupEditModal .modal-content').load('/Group/EditGroupModal/' + id);
        $('#GroupEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除
    function deleteGroup(groupId, groupName) {
        window.swal({
            title: '确定删除' + groupName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/Group/Delete/' + groupId,
                    function (result) {
                        if (result.success) {
                            window.swal('操作成功', groupName + '已被删除!', 'success');
                            refresh();
                        } else {
                            window.swal('操作失败', result.message, 'error');
                        }
                    });
            }
        });
    }

    //清空表单
    function resetForm() {
        $('#GroupCreateForm')[0].reset();
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
        if (!Common.hasOperation('Group.Create')) {
            $('#btnAddGroup').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();

