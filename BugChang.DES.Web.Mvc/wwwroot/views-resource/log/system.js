var LogSystem = function () {

    var table;

    $(function () {

        //toastr提示2s自动关闭
        window.toastr.options.timeOut = 2000;

        //初始化table
        initTable();

        $('table').delegate('.show-data',
            'click',
            function () {
                var rowIndex = $(this).attr('data-rowindex');
                showData(rowIndex);
            });


        $('.search-time').datetimepicker({
            format: 'yyyy-mm-dd',
            language: 'zh-CN',
            autoclose: true,
            minView: 2
        });

        $("#btnSearch").click(function() {
            table.ajax.reload();
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
                url: '/Log/GetSystemLogs',
                data: function (d) {
                    d.Level = $('#Level').val();
                    d.Title = $('#Title').val();
                    d.Content = $('#Content').val();
                    d.BeginTime = $('#BeginTime').val();
                    d.EndTime = $('#EndTime').val();
                }
            },
            stateSave: true,
            columns: [
                {
                    data: 'id',
                    title: '主键'

                },
                {
                    data: 'level',
                    title: '级别'
                },
                {
                    data: 'title',
                    title: '标题'
                },
                {
                    data: 'content',
                    title: '内容'
                },
                {
                    data: 'createTime',
                    title: '时间'
                },
                {
                    data: 'data',
                    title: '数据'
                }
            ],
            columnDefs: [
                {
                    targets: 1,
                    render: function (data, type, row) {
                        var labelclass = '';
                        switch (row.level) {
                            case '调试':
                                labelclass = 'label-primary';
                                break;
                            case '信息':
                                labelclass = 'label-info';
                                break;
                            case '警告':
                                labelclass = 'label-warning';
                                break;
                            case '错误':
                                labelclass = 'label-error';
                                break;
                            case '致命':
                                labelclass = 'label-error';
                                break;
                            default:
                                labelclass = '';
                                break;
                        }
                        var strHtml = '<label class="label ' + labelclass + '"> ' + row.level + '</label>&nbsp;';
                        return strHtml;
                    }
                },
                {
                    targets: 5,
                    render: function (data, type, row, meta) {
                        if (!row.data) {
                            return '-';
                        }
                        var strHtml = '<button class="btn btn-primary btn-xs show-data"  data-rowindex="' + meta.row + '">查看数据</button>&nbsp;';
                        return strHtml;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

    //展示Json数据
    function showData(rowIndex) {
        var data = table.rows(rowIndex).data()[0];
        var result = JSON.stringify(JSON.parse(data.data), null, 4);
        Common.alert(result);
    }
}();

