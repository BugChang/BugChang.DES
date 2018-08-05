(function () {
    var table;
    $(function () {

        initReceiveDepartments();

        initTable();

        $('.search-time').datetimepicker({
            format: 'yyyy-mm-dd hh:ii',
            language: 'zh-CN'
        });

        $("#btnSearch").click(function () {
            search();
        });
    });



    //初始化table
    function initTable() {
        table = $('#table').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            searching: false,
            ajax: {
                url: '/Letter/GetSendLetters',
                data: function (d) {
                    d.letterNo = $('#LetterNo').val();
                    d.receiveDepartmentId = $('.department-select').val();
                    d.beginTime = $('#BeginTime').val();
                    d.endTime = $('#EndTime').val();
                }
            },
            stateSave: true,
            columns: [
                {
                    data: 'letterNo',
                    title: '信封编号'
                },
                {
                    data: 'receiveDepartmentName',
                    title: '收件单位'
                },
                {
                    data: 'receiver',
                    title: '收件人'
                },
                {
                    data: 'secretLevel',
                    title: '秘密等级'
                },
                {
                    data: 'urgencyLevel',
                    title: '缓急程度'
                },
                {
                    data: 'urgencyTime',
                    title: '限时时间'
                },
                {
                    data: 'customData',
                    title: '附加数据'
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
                    data: null,
                    title: '操作'
                }
            ],
            columnDefs: [
                {
                    targets: 3,
                    render: function (data, type, row) {
                        var secretLevelText;
                        switch (row.secretLevel) {

                            case 0:
                                secretLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                secretLevelText = '<label class="label label-info"> 秘密</label>';
                                break;
                            case 2:
                                secretLevelText = '<label class="label label-warning">机密</label>';
                                break;
                            case 3:
                                secretLevelText = '<label class="label label-danger"> 绝密</label>';
                                break;
                            default:
                                secretLevelText = "未知";
                                break;
                        }
                        return secretLevelText;
                    }
                },
                {
                    targets: 4,
                    render: function (data, type, row) {
                        var secretLevelText;
                        switch (row.urgencyLevel) {

                            case 0:
                                secretLevelText = '<label class="label label-default"> 无</label>';
                                break;
                            case 1:
                                secretLevelText = '<label class="label label-info"> 紧急</label>';
                                break;
                            case 2:
                                secretLevelText = '<label class="label label-warning">特急</label>';
                                break;
                            case 3:
                                secretLevelText = '<label class="label label-danger"> 限时</label>';
                                break;
                            default:
                                secretLevelText = "未知";
                                break;
                        }
                        return secretLevelText;
                    }
                },
                {
                    targets: 9,
                    render: function (data, type, row) {
                        var strHtml = '';
                        strHtml += '<button class="btn btn-primary btn-xs exchange-detail" data-letter-id=' + row.id + '>流转详情</button>&nbsp;';
                        return strHtml;
                    }
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }


    function search() {
        table.ajax.reload();
    }

    function initReceiveDepartments() {
        $.get('/Letter/GetDepartments',
            function (data) {
                $('.department-select').select2({
                    data: data,
                    allowClear: false
                });
            });
    }
})();