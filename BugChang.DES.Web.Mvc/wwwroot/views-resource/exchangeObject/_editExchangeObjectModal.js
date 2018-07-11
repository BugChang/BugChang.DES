(function () {
    $(function () {

        initObjectTypeSelect();

        $("#ExchangeObjectEditForm").submit(function (e) {
            e.preventDefault();
            var valueText = $('.edit-object-value-select').select2("data")[0].text;
            $("#ValueText").val(valueText);
            var data = $(this).serialize();
            $.post('/ExchangeObject/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#ExchangeObjectEditModal").modal("hide");
                        //刷新父页面
                        ExchangeObjectIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });

        $(".edit-object-type-select").on("select2:select",
            function () {
                initObjectValueSelect();
            });
    });

    //初始化机构列表
    function initObjectTypeSelect() {
        $.get('/ExchangeObject/GetObjectTypes',
            function (data) {
                $('.edit-object-type-select').select2({
                    data: data,
                    allowClear: false
                });
                var objectType = $('#DefaultValue').attr('data-object-type');
                $('.edit-object-type-select').val(objectType).trigger("change");
                initObjectValueSelect();
            });
    }

    //初始化上级交换场所列表
    function initObjectValueSelect() {
        var objectType = $('.edit-object-type-select').val();
        $.get('/ExchangeObject/GetValuesByObjectType',
            { objectType: objectType },
            function (data) {
                $('.edit-object-value-select').html("");
                $('.edit-object-value-select').select2({
                    data: data,
                    allowClear: false
                });
                var objectValue = $('#DefaultValue').attr('data-object-value');
                $('.edit-object-value-select').val(objectValue).trigger("change");
            });
    }
})();