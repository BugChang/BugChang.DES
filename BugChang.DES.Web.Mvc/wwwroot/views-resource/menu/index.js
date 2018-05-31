var MenuIndex = function () {
    var currentNode = null;
    var table;
    var zTreeObj;

    var setting = { // zTree 参数配置
        async: {
            enable: true,
            url: '/Menu/GetTreeData',
            autoParam: ['id=parentId'],
            type: 'get'
        },
        callback: {
            beforeClick: zTreeBeforeClick
        }
    };

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化zTree
        zTreeObj = $.fn.zTree.init($('#menuTree'), setting);

        //初始化table
        initTable();

        //初始化select2选择框
        initSelect();

        //新增机构表单提交时
        $('#MenuCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Menu/Edit', data, function (result) {
                if (result.success) {
                    resetForm();
                    //关闭模态
                    $('#MenuCreateModal').modal('hide');
                    //刷新页面
                    refresh();
                    window.toastr.success('操作成功');
                } else {
                    window.toastr.error(result.message);
                }
            })
        });

        $('table').delegate('.view-menu',
            'click',
            function () {
                var menuId = $(this).attr('data-menu-id');
                viewMenu(menuId);
            });

        $('table').delegate('.edit-menu',
            'click',
            function () {
                var menuId = $(this).attr('data-menu-id');
                editMenu(menuId);
            });

        $('table').delegate('.delete-menu',
            'click',
            function () {
                var menuId = $(this).attr('data-menu-id');
                var menuName = $(this).attr('data-menu-name');
                deleteMenu(menuId, menuName);
            });

    });

    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode) {
        currentNode = treeNode;
        if (!currentNode.customData) {
            showAddButton(true);
        } else {
            showAddButton(false);
        }
        table.ajax.reload();
        $('.select2').val(treeNode.id).trigger('change');
    }


    //初始化table
    function initTable() {
        table = $('#table').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: '/Menu/GetListForTable',
                data: function (para) {
                    //添加额外的参数传给服务器
                    para.parentId = currentNode === null ? null : currentNode.id;
                }
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
                    data: 'url',
                    title: '地址'
                },
                {
                    data: 'description',
                    title: '描述'
                },
                {
                    data: 'icon',
                    title: '图标'
                },
                {
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 5,
                    render: function (data, type, row) {
                        var strHtml =
                            '<button class="btn btn-info btn-xs view-menu" data-menu-id=' + row.id + '>查看</button>&nbsp;' +
                            '<button class="btn btn-warning btn-xs edit-menu" data-menu-id=' + row.id + '>修改</button>&nbsp;' +
                            '<button class="btn btn-danger btn-xs delete-menu" data-menu-id=' + row.id + ' data-menu-name=' + row.name + '>删除</button>';
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
        $.get('/Menu/GetListForSelect',
            function (data) {
                $('.select2').select2({
                    data: data,
                    placeholder: '请选择上级菜单',
                    allowClear: true
                });
            });
    }

    //查看菜单详情
    function viewMenu(id) {
        alert('查看' + id);
    }

    //编辑菜单信息
    function editMenu(id) {
        $('#MenuEditModal .modal-content').load('/Menu/EditMenuModal/' + id);
        $('#MenuEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除菜单
    function deleteMenu(menuId, menuName) {
        window.swal({
            title: '确定删除' + menuName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/Menu/Delete/' + menuId,
                    function (result) {
                        if (result.success) {
                            window.swal('操作成功', menuName + '已被删除!', 'success');
                            refresh();
                        } else {
                            window.swal('操作失败', result.message, 'error');
                        }
                    }).error(function () {
                        window.swal('操作失败', "111", 'error');
                    });
            }
        });
    }

    //控制新增按钮的显示
    function showAddButton(isShow) {
        if (isShow) {
            $('#btnAddMenu').show();
        } else {
            $('#btnAddMenu').hide();
        }
    }

    //清空表单
    function resetForm() {
        $('#MenuCreateForm')[0].reset();
        $('.select2').val(currentNode.id).trigger('change');
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.ajax.reload();
        //刷新菜单树
        zTreeObj.reAsyncChildNodes(currentNode, 'refresh');
    }

    //向外暴露方法
    return { refresh: refresh };
}();

