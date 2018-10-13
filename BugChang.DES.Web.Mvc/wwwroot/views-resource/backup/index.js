var BackUpIndex = function () {
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

        $('#btnRefresh').click(function () {
            reload();
        });

        $("#btnBackUpNow").click(function () {
            window.swal(
                {
                    title: "请输入备份说明",
                    buttons: ["取消", "确认"],
                    content: "input"
                })
                .then((value) => {
                    if (value !== null) {
                        if ($.trim(value) === '') {
                            window.toastr.error('备份说明不能为空');
                        } else {
                            var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
                            $.ajax({
                                type: 'POST',
                                async: false,
                                cache: false,
                                data: { remark: value },
                                headers:
                                {
                                    "BugChang-CSRF-HEADER": token //注意header要修改
                                },
                                url: "/BackUp/BackUpNow",
                                success: function (result) {
                                    if (result.success) {
                                        window.toastr.success('备份成功');
                                        refresh();
                                    } else {
                                        window.toastr.error('备份失败');
                                    }
                                }
                            });
                        }
                    }
                });
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
                url: '/BackUp/GetBackUps'
            },
            stateSave: true,
            columns: [
                {
                    data: 'id',
                    title: '主键'
                },
                {
                    data: 'fileName',
                    title: '文件名'
                },
                {
                    data: 'md5',
                    title: 'MD5'
                },
                {
                    data: 'dateTime',
                    title: '备份时间'
                },
                {
                    data: 'operatorName',
                    title: '操作人'
                },
                {
                    data: 'type',
                    title: '类型'
                },
                {
                    data: 'remark',
                    title: '备份说明'
                }
            ],
            columnDefs: [
                {
                    targets: 5,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (row.type === 1) {
                            strHtml = "自动备份";
                        } else {
                            strHtml = "手动备份";
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
        if (!Common.hasOperation('Place.Create')) {
            $('#btnAddPlace').hide();
        }
    }

    //向外暴露方法
    return { refresh: refresh };
}();
