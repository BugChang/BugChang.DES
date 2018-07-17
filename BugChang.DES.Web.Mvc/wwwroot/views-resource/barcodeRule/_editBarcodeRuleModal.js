(function () {
    $(function () {

        initBarcodeType();

        $("#BarcodeRuleEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/BarcodeRule/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#BarcodeRuleEditModal").modal("hide");
                        //刷新父页面
                        BarcodeRuleIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    //初始化条码类型
    function initBarcodeType() {
        $.get('/BarcodeRule/GetBarcodeTypes',
            function (data) {
                $('.edit-barcode-type-select').select2({
                    data: data,
                    allowClear: false
                });
                var barcodeType = $('#DefaultValue').attr('data-barcode-type');
                $('.edit-object-type-select').val(barcodeType).trigger("change");
            });
    }
})();