var ExchangeObjectIndex = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('ExchangeObject');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //初始化流转对象类型
        initObjectTypes();

        //新增角色表单提交
        $('#ExchangeObjectCreateForm').submit(function (e) {
            e.preventDefault();
            var valueText = $('.object-value-select').select2("data")[0].text;
            $('#createValueText').val(valueText);
            var data = $(this).serialize();
            try {
                $.post('/ExchangeObject/Create',
                    data,
                    function (result) {
                        if (result.success) {
                            resetForm();
                            //关闭模态
                            $('#ExchangeObjectCreateModal').modal('hide');
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

        $('table').delegate('.edit-exchange-object',
            'click',
            function () {
                var objectId = $(this).attr('data-object-id');
                editExchangeObject(objectId);
            });

        $('table').delegate('.delete-exchange-object',
            'click',
            function () {
                var objectId = $(this).attr('data-object-id');
                var objectName = $(this).attr('data-object-name');
                deleteExchangeObject(objectId, objectName);
            });

        $('#btnRefresh').click(function () {
            reload();
        });

        $(".object-type-select").on("select2:select",
            function () {
                initValues();
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
                url: '/ExchangeObject/GetExchangeObjects'
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
                    data: 'objectType',
                    title: '对象类型'
                },
                {
                    data: 'valueText',
                    title: '值'
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
                        if (Common.hasOperation('ExchangeObject.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-exchange-object" data-object-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('ExchangeObject.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-exchange-object" data-object-id=' + row.id + ' data-object-name=' + row.name + '>删除</button>';
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

    //初始化流转对象值
    function initValues() {
        var objectType = $('.object-type-select').val();
        $.get('/ExchangeObject/GetValuesByObjectType',
            { objectType: objectType },
            function (data) {
                $('.object-value-select').html("");
                $('.object-value-select').select2({
                    data: data,
                    allowClear: false
                });
            });
    }

    //初始化流转对象类型
    function initObjectTypes() {
        $.get('/ExchangeObject/GetObjectTypes',
            function (data) {
                $('.object-type-select').select2({
                    data: data,
                    allowClear: false
                });
                initValues();
            });
    }


    //编辑流转对象
    function editExchangeObject(id) {
        $('#ExchangeObjectEditModal .modal-content').load('/ExchangeObject/EditExchangeObjectModal/' + id);
        $('#ExchangeObjectEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除角色
    function deleteExchangeObject(objectId, objectName) {
        window.swal({
            title: '确定删除' + objectName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/ExchangeObject/Delete/' + objectId,
                    function (result) {
                        if (result.success) {
                            window.swal('操作成功', objectName + '已被删除!', 'success');
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
        $('#ExchangeObjectCreateForm')[0].reset();
        $('.object-type-select').trigger("change");
        initValues();
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
        if (!Common.hasOperation('ExchangeObject.Create')) {
            $('#btnAddExchangeObject').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();

