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

        window.toastr.options.timeOut = 2000;

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
            $(this).ajaxSubmit({
                type: 'post',
                url: '/Department/Edit',
                data: data,
                success: function (result) {
                    if (result.success) {
                        resetForm();
                        //关闭模态
                        $('#DepartmentCreateModal').modal('hide');
                        //刷新页面
                        refresh();
                        window.toastr.success('操作成功');
                    } else {
                        //toastr.success('We do have the Kapua suite available.', 'Turtle Bay Resort', { timeOut: 5000 });
                        window.toastr.error(result.message);
                    }

                }
            });
        });
        $('#btnLoadRoot').click(function() {
            currentNode = null;
            refresh();
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
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 4,
                    render: function (data, type, row) {
                        var strHtml =
                            '<button class="btn btn-info btn-xs view-department" data-department-id=' + row.id + '>查看</button>&nbsp;' +
                            '<button class="btn btn-warning btn-xs edit-department" data-department-id=' + row.id + '>修改</button>&nbsp;' +
                            '<button class="btn btn-danger btn-xs delete-department" data-department-id=' + row.id + ' data-department-name=' + row.name + '>删除</button>';
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
        alert('查看' + id);
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
        $('#DepartmentCreateForm').resetForm();
        $('.select2').val(currentNode.id).trigger('change');
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.ajax.reload();
        //刷新机构树
        zTreeObj.reAsyncChildNodes(currentNode, 'refresh');
    }

    //向外暴露方法
    return { refresh: refresh };
}();

