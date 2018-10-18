(function () {
    var logTable;
    var barcodeNo = $("#DefaultValue").attr("data-barcode-no");
    $(function () {
        initTable();
    });

    function initTable() {
        logTable = $('#logTable').DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            searching: false,
            ajax: {
                url: '/Letter/GetSortingListDetailsForTable',
                data: function (para) {
                    //添加额外的参数传给服务器
                    para.barcodeNo = barcodeNo;
                }
            },
            stateSave: true,
            columns: [
                {
                    data: 'barcodeNumber',
                    title: '条码号'
                },
                {
                    data: 'barcodeStatus',
                    title: '条码状态'
                },
                {
                    data: 'barcodeSubStatus',
                    title: '条码子状态'
                },
                {
                    data: 'lastOperationTime',
                    title: '上个操作时间'
                },
                {
                    data: 'operationTime',
                    title: '操作时间'
                },
                {
                    data: 'departmentName',
                    title: '单位名称'
                },
                {
                    data: 'operatorName',
                    title: '操作人'
                },
                {
                    data: 'currentObjectName',
                    title: '当前流转对象'
                },
                {
                    data: 'currentPlaceName',
                    title: '当前场所'
                },
                {
                    data: 'remark',
                    title: '备注'
                },
                {
                    data: 'isSynBill',
                    title: '清单打印状态'
                }
            ],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

})();