var ClientIndex = function () {
    var table;
    var socket;
    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('Client');

        //初始化页面元素
        initPageElement();

        initSocket();

        //初始化table
        initTable();

        //初始化条码类型
        initPlaces();

        //新增
        $('#ClientCreateForm').submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            try {
                $.post('/Client/Create',
                    data,
                    function (result) {
                        if (result.success) {
                            resetForm();
                            //关闭模态
                            $('#ClientCreateModal').modal('hide');
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

        $('table').delegate('.edit-client',
            'click',
            function () {
                var clientId = $(this).attr('data-client-id');
                editClient(clientId);
            });

        $('table').delegate('.delete-client',
            'click',
            function () {
                var clientId = $(this).attr('data-client-id');
                var clientName = $(this).attr('data-client-name');
                deleteClient(clientId, clientName);
            });


        $('#btnRefresh').click(function () {
            reload();
        });

        $(".btn-device-code").click(function () {
            getDeviceCode();
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
                url: '/Client/GetClients'
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
                    data: 'clientType',
                    title: '客户端类型'
                },
                {
                    data: 'placeName',
                    title: '交换场所'
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
                        if (Common.hasOperation('Client.Edit')) {
                            strHtml += '<button class="btn btn-info btn-xs edit-client" data-client-id=' + row.id + '>修改</button>&nbsp;';
                        }
                        if (Common.hasOperation('Client.Delete')) {
                            strHtml += '<button class="btn btn-danger btn-xs delete-client" data-client-id=' + row.id + ' data-client-name=' + row.name + '>删除</button>';
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
    function initPlaces() {
        $.get('/Client/GetPlaces',
            function (data) {
                $('.place-select').select2({
                    data: data,
                    allowClear: false
                });
            });
    }

    function initSocket() {
        if (typeof (WebSocket) === "undefined") {
            window.toastr.error("您的浏览器不支持WebSocket");
        }

        socket = new WebSocket("ws://localhost:8181");

        socket.onopen = function () {
            window.toastr.success("SuperService已连接");
        };
        socket.onmessage = function (e) {
            var obj = JSON.parse(e.data);
            if (obj.Method === "GetMacAddress") {
                $(".device-code").val(obj.Data);
            }
        };
        socket.onerror = function (e) {
            window.toastr.error("与SuperService连接出现错误");
        };
        socket.onclose = function () {
            window.toastr.error("SuperService连接已关闭");
        };
    }
    //编辑
    function editClient(id) {
        $('#ClientEditModal .modal-content').load('/Client/EditClientModal/' + id);
        $('#ClientEditModal').modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    }

    //删除
    function deleteClient(clientId, clientName) {
        window.swal({
            title: '确定删除' + clientName + '?',
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
                    url: "/Client/Delete/" + clientId,
                    success: function (result) {
                        if (result.success) {
                            window.swal('操作成功', clientName + '已被删除!', 'success');
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
        $('#ClientCreateForm')[0].reset();
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
        if (!Common.hasOperation('Client.Create')) {
            $('#btnAddClient').hide();
        }
    }

    //获取设备编码
    function getDeviceCode() {
        var getMacAddress = { command: 'GetMacAddress' };
        socket.send(JSON.stringify(getMacAddress));
    }

    //向外暴露
    return {
        refresh: refresh,
        getDeviceCode: getDeviceCode
    };
}();

