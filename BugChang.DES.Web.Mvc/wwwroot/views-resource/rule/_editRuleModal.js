(function () {
    $(function () {

        initType();

        $("#RuleEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Rule/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#RuleEditModal").modal("hide");
                        //刷新父页面
                        RuleIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    //初始化条码类型
    function initType() {
        $.get('/Rule/GetBarcodeTypes',
            function (data) {
                $('.edit-barcode-type-select').select2({
                    data: data,
                    allowClear: false
                });
                var barcodeType = $('#DefaultValue').attr('data-barcode-type');
                $('.edit-barcode-type-select').val(barcodeType).trigger("change");
            });
    }
})();