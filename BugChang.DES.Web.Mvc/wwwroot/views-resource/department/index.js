var DepartmentIndex = function () {
    var currentNode = null;
    var table;
    var zTreeObj;


    var setting = { // zTree 参数配置
        async: {
            enable: true,
            url: '/Department/GetTreeData',
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

        //初始化操作代码
        Common.initOperations('Department');

        //初始化页面元素
        initPageElement();

        //初始化zTree
        zTreeObj = $.fn.zTree.init($('#departmentTree'), setting);

        //初始化table
        initTable();

        //初始化select2选择框
        initSelect();

        //新增机构表单提交时
        $('#DepartmentCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Department/Create', data,
                function (result) {
                    if (result.success) {
                        resetForm();
                        //关闭模态
                        $('#DepartmentCreateModal').modal('hide');
                        //刷新页面
                        refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        $('table').delegate('.view-department',
            'click',
            function () {
                var departmentId = $(this).attr('data-department-id');
                viewDepartment(departmentId);
            });

        $('table').delegate('.edit-department',
            'click',
            function () {
                var departmentId = $(this).attr('data-department-id');
                editDepartment(departmentId);
            });

        $('table').delegate('.delete-department',
            'click',
            function () {
                var departmentId = $(this).attr('data-department-id');
                var departmentName = $(this).attr('data-department-name');
                deleteDepartment(departmentId, departmentName);
            });
        $('#btnRefresh').click(function () {
            reload();
        });


    });



    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode) {
        currentNode = treeNode;
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
                url: '/Department/GetListForTable',
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
                    data: 'fullName',
                    title: '全称'
                },
                {
                    data: 'code',
                    title: '代码'
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
                    title: '最后更新人'
                },
                {
                    data: 'updateTime',
                    title: '最后更新时间'
                },
                {
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 8,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation("Department.Edit")) {
                            strHtml += '<button class="btn btn-info btn-xs edit-department" data-department-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation("Department.Delete")) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-department" data-department-id=' + row.id + ' data-department-name=' + row.name + '>删除</button>';
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

    //初始化上级机构
    function initSelect() {
        $.get('/Department/GetListForSelect',
            function (data) {
                $('.select2').select2({
                    data: data,
                    placeholder: '请选择上级机构',
                    allowClear: true
                });
            });
    }

    //查看机构详情
    function viewDepartment(id) {
        $('#DepartmentViewModal .modal-content').load('/Department/ViewDepartmentModal/' + id);
        $('#DepartmentViewModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //编辑机构信息
    function editDepartment(id) {
        $('#DepartmentEditModal .modal-content').load('/Department/EditDepartmentModal/' + id);
        $('#DepartmentEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除机构
    function deleteDepartment(departmentId, departmentName) {
        window.swal({
            title: '确定删除' + departmentName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/Department/Delete/' + departmentId,
                    function (result) {
                        if (result.success) {
                            window.swal('操作成功', departmentName + '已被删除!', 'success');
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
        $('#DepartmentCreateForm')[0].reset();
        $('.select2').val(currentNode.id).trigger('change');
    }

    //刷新数据
    function refresh() {
        //刷新表格
        table.ajax.reload();
        currentNode.isParent = true;
        zTreeObj.updateNode(currentNode);
        //刷新机构树
        zTreeObj.reAsyncChildNodes(currentNode, 'refresh');
        //刷新上级单位列表
        initSelect();
    }

    //重新加载页面
    function reload() {
        window.location.reload();
    }

    //初始化页面元素
    function initPageElement() {
        if (!Common.hasOperation('Department.Create')) {
            $('#btnAddDepartment').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();

