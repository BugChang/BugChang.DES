(function () {
    var table;
    $(function () {

        initTable();

    });

    function initTable() {
        table = $('#table').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: '/Letter/Out2InsideLetters'
            },
            stateSave: true,
            dom: '<"top"i>rt<"bottom"flp><"clear">',
            columns: [
                {
                    data: 'letterNo',
                    title: '信封编号'
                },
                {
                    data: 'oldBarcodeNo',
                    title: '原条码号'
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
                    data: 'sendDepartmentName',
                    title: '发件单位'
                },
                {
                    data: 'oldSendDepartmentName',
                    title: '原发件单位'
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
                    data: 'shiJiCode',
                    title: '市机码'
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
                }
            ],
            columnDefs: [
                {
                    targets: 6,
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
                    targets: 7,
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
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }
})();