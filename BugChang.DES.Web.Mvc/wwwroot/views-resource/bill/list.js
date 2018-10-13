var BillList = function () {
    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化操作代码
        Common.initOperations('Bill');

        //初始化table
        initTable();

        $('table').delegate('.view-detail',
            'click',
            function () {
                var billId = $(this).attr('data-bill-id');
                location.href = "/Bill/Detail/" + billId;
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
                url: '/Bill/GetBills'
            },
            stateSave: true,
            columns: [
                {
                    data: 'objectName',
                    title: '交换对象'
                },
                {
                    data: 'listNo',
                    title: '清单号'
                },
                {
                    data: 'type',
                    title: '清单类型'
                },
                {
                    data: 'exchangeUserName',
                    title: '交换员'
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
                        strHtml += '<button class="btn btn-primary btn-xs view-detail" data-bill-id=' + row.id + '>详情</button>';
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
        table.draw(false);
    }

    //重新加载页面
    function reload() {
        window.location.reload();
    }

    //向外暴露方法
    return { refresh: refresh };
}();

