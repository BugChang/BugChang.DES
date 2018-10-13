var CardIndex = function () {
    var table;
    var socket;
    var deviceCode;
    $(function () {
        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        initSocket();

        //初始化操作代码
        Common.initOperations('Card');

        //初始化页面元素
        initPageElement();

        //初始化table
        initTable();

        //初始化条码类型
        initUsers();

        //新增
        $('#CardCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            try {
                $.post('/Card/Create',
                    data,
                    function (result) {
                        if (result.success) {
                            resetForm();
                            //关闭模态
                            $('#CardCreateModal').modal('hide');
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

        $('table').delegate('.edit-card',
            'click',
            function () {
                var cardId = $(this).attr('data-card-id');
                editCard(cardId);
            });

        $('table').delegate('.delete-card',
            'click',
            function () {
                var cardId = $(this).attr('data-card-id');
                var cardNo = $(this).attr('data-card-no');
                deleteCard(cardId, cardNo);
            });
        $('table').delegate('.change-enabled',
            'click',
            function () {
                var cardId = $(this).attr('data-card-id');
                changeEnabled(cardId);

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
                url: '/Card/GetCards'
            },
            stateSave: true,
            columns: [
                {
                    data: 'id',
                    title: '主键'

                },
                {
                    data: 'userName',
                    title: '用户'
                },
                {
                    data: 'number',
                    title: '编号'
                },
                {
                    data: 'value',
                    title: '卡号'
                },
                {
                    data: 'enabled',
                    title: '启用状态'
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
                        var strHtml = '';
                        if (Common.hasOperation('Card.Enabled')) {
                            if (row.enabled) {
                                strHtml += '<button class="btn btn-danger btn-xs change-enabled" data-card-id=' + row.id + '>禁用</button>';
                            } else {
                                strHtml += '<button class="btn btn-success btn-xs change-enabled" data-card-id=' + row.id + '>启用</button>';
                            }
                        } else {
                            if (row.enabled) {
                                strHtml = '<label class="label label-success">已启用</label>';
                            } else {
                                strHtml = '<label class="label label-danger">已禁用</label>';
                            }
                        }

                        return strHtml;
                    }
                },
                {
                    targets: 9,
                    render: function (data, type, row) {
                        var strHtml = '';
                        if (Common.hasOperation('Card.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-card" data-card-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('Card.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-card" data-card-id=' + row.id + ' data-card-no=' + row.number + '>删除</button>';
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

    //初始化用户下拉列表
    function initUsers() {
        $.get('/Card/GetUsers',
            function (data) {
                $('.user-select').select2({
                    data: data,
                    allowClear: false
                });
            });
    }


    //编辑
    function editCard(id) {
        $('#CardEditModal .modal-content').load('/Card/EditCardModal/' + id);
        $('#CardEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除
    function deleteCard(cardId, cardName) {
        window.swal({
            title: '确定删除' + cardName + '?',
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
                    url: "/Card/Delete/" + cardId,
                    success: function (result) {
                        if (result.success) {
                            window.swal('操作成功', cardName + '已被删除!', 'success');
                            refresh();
                        } else {
                            window.swal('操作失败', result.message, 'error');
                        }
                    }
                });
            }
        });
    }

    //启用更改
    function changeEnabled(id) {
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
            url: "/Card/ChangeEnabled/" + id,
            success: function (result) {
                if (result.success) {
                    //刷新页面
                    refresh();
                    window.toastr.success('操作成功');
                } else {
                    window.toastr.error(result.message);
                }
            }
        });
    }

    //清空表单
    function resetForm() {
        $('#CardCreateForm')[0].reset();
    }

    //刷新页面
    function refresh() {
        //刷新表格
        table.draw(false);
    }

    //重新加载页面
    function reload() {
        window.location.reload();
    }

    //初始化页面元素
    function initPageElement() {
        if (!Common.hasOperation('Card.Create')) {
            $('#btnAddCard').hide();
        }
    }

    function initSocket() {
        if (typeof (WebSocket) === "undefined") {
            window.toastr.error("您的浏览器不支持WebSocket");
        }

        socket = new WebSocket("ws://localhost:8181");

        socket.onopen = function () {
            var getMacAddress = { command: 'GetMacAddress' };
            socket.send(JSON.stringify(getMacAddress));
        };
        socket.onmessage = function (e) {
            var obj = JSON.parse(e.data);
            if (obj.Method === "GetMacAddress") {
                deviceCode = obj.Data;
                $("#DeviceCode").val(deviceCode);
                openCpuCom();
            }
            if (obj.Method === "GetCpuCardNo") {
                if (obj.data==="") {
                    return false;
                }
                var d1 = obj.Data.substr(0, 2);
                var d2 = obj.Data.substr(2, 2);
                var d3 = obj.Data.substr(4, 2);
                var d4 = obj.Data.substr(6, 2);
                var cardValue = d4 + d3 + d2 + d1;
                $("#addValue").val(cardValue);
            }

        };
        socket.onerror = function (e) {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }

    function openCpuCom() {
        $.get("/HardWare/GetCpuReadCard",
            { deviceCode: deviceCode },
            function (data) {
                var row = { command: 'OpenCpuCom', port: data.value.replace("COM", ""), rate: data.baudRate };
                socket.send(JSON.stringify(row));
            });
    }

    //向外暴露方法
    return { refresh: refresh };
}();

