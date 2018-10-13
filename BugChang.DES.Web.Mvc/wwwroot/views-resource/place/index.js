var ExchangePlace = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('Place');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //初始化select2选择框
        initDepartmentSelect();
        initParentPlaceSelect();

        //新增菜单表单提交
        $('#PlaceCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Place/Create',
                data,
                function (result) {
                    if (result.success) {
                        resetForm();
                        //关闭模态
                        $('#PlaceCreateModal').modal('hide');
                        //刷新页面
                        refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        $('table').delegate('.edit-place',
            'click',
            function () {
                var placeId = $(this).attr('data-place-id');
                editPlace(placeId);
            });

        $('table').delegate('.delete-place',
            'click',
            function () {
                var placeId = $(this).attr('data-place-id');
                var name = $(this).attr('data-place-name');
                deletePlace(placeId, name);
            });
        $('table').delegate('.assign-warden',
            'click',
            function () {
                var placeId = $(this).attr('data-place-id');
                assignWarden(placeId);
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
                url: '/Place/GetPlaces'
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
                    data: 'departmentName',
                    title: '所属机构'
                },
                {
                    data: 'parentName',
                    title: '上级交换场所'
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
                    targets: 8,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation('Place.AssignWarden')) {
                            strHtml += '<button class="btn btn-primary btn-xs assign-warden" data-place-id=' + row.id + '>分配管理员</button>&nbsp;';
                        }
                        if (Common.hasOperation('Place.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-place" data-place-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('Place.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-place" data-place-id=' + row.id + ' data-place-name=' + row.name + '>删除</button>';
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

    //初始化机构列表
    function initDepartmentSelect() {
        $.get('/Place/GetDepartmentsForSelect',
            function (data) {
                $('.department-select').select2({
                    data: data,
                    placeholder: '请选择机构',
                    allowClear: false
                });
            });
    }

    //初始化上级交换场所列表
    function initParentPlaceSelect() {
        $.get('/Place/GetPlacesForSelect',
            function (data) {
                $('.parent-select').select2({
                    data: data,
                    placeholder: '请选择上级交换场所',
                    allowClear: false
                });
            });
    }

    //分配场所管理员
    function assignWarden(placeId) {
        $('#AssignWardenModal .modal-content').load('/Place/AssignWardenModal/' + placeId);
        $('#AssignWardenModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //编辑交换场所
    function editPlace(id) {
        $('#PlaceEditModal .modal-content').load('/Place/EditPlaceModal/' + id);
        $('#PlaceEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除菜单
    function deletePlace(placeId, placeName) {
        window.swal({
            title: '确定删除' + placeName + '?',
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
                    url: "/Place/Delete/" + placeId,
                    success: function (result) {
                        if (result.success) {
                            window.swal('操作成功', placeName + '已被删除!', 'success');
                            refresh();
                        } else {
                            window.swal('操作失败', result.message, 'error');
                        }
                    }
                });
            }
        });
    }

    //清空表单
    function resetForm() {
        $('#PlaceCreateForm')[0].reset();
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.ajax.reload();

        initParentPlaceSelect();
    }

    //重新加载页面
    function reload() {
        window.location.reload();
    }

    //初始化页面元素
    function initPageElement() {
        if (!Common.hasOperation('Place.Create')) {
            $('#btnAddPlace').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();
