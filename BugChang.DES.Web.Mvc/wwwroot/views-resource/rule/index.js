var RuleIndex = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('Rule');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //初始化条码类型
        initTypes();

        //新增
        $('#RuleCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            try {
                $.post('/Rule/Create',
                    data,
                    function (result) {
                        if (result.success) {
                            resetForm();
                            //关闭模态
                            $('#RuleCreateModal').modal('hide');
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

        $('table').delegate('.edit-rule',
            'click',
            function () {
                var ruleId = $(this).attr('data-rule-id');
                editRule(ruleId);
            });

        $('table').delegate('.delete-rule',
            'click',
            function () {
                var ruleId = $(this).attr('data-rule-id');
                var ruleName = $(this).attr('data-rule-name');
                deleteRule(ruleId, ruleName);
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
                url: '/Rule/GetRules'
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
                    data: 'barcodeType',
                    title: '条码类型'
                },
                {
                    data: 'noRegisterSend',
                    title: '不登记投箱'
                },
                {
                    data: 'isAnalysisDepartment',
                    title: '解析单位'
                },
                {
                    data: 'analysisDepartmentLevel',
                    title: '解析单位级别'
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
                        var strHtml;
                        if (row.noRegisterSend) {
                            strHtml = '<label class="label label-success">是</label>&nbsp;';
                        } else {
                            strHtml = '<label class="label label-danger">否</label>&nbsp;';
                        }
                        return strHtml;
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        var strHtml;
                        if (row.isAnalysisDepartment) {
                            strHtml = '<label class="label label-success">是</label>&nbsp;';
                        } else {
                            strHtml = '<label class="label label-danger">否</label>&nbsp;';
                        }
                        return strHtml;
                    }
                },
                {
                    targets: 10,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation('Rule.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-rule" data-rule-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('Rule.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-rule" data-rule-id=' + row.id + ' data-rule-name=' + row.name + '>删除</button>';
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

    //初始化条码类型
    function initTypes() {
        $.get('/Rule/GetBarcodeTypes',
            function (data) {
                $('.barcode-type-select').select2({
                    data: data,
                    allowClear: false
                });
            });
    }


    //编辑
    function editRule(id) {
        $('#RuleEditModal .modal-content').load('/Rule/EditRuleModal/' + id);
        $('#RuleEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除
    function deleteRule(ruleId, ruleName) {
        window.swal({
            title: '确定删除' + ruleName + '?',
            //text: '删除后无法恢复数据!',
            icon: 'warning',
            buttons: ['取消', '确定'],
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.post('/Rule/Delete/' + ruleId,
                    function (result) {
                        if (result.success) {
                            window.swal('操作成功', ruleName + '已被删除!', 'success');
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
        $('#RuleCreateForm')[0].reset();
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
        if (!Common.hasOperation('Rule.Create')) {
            $('#btnAddRule').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();

