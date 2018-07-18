(function () {
    $(function () {

        initTypes();

        $("#GroupEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Group/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#GroupEditModal").modal("hide");
                        //刷新父页面
                        GroupIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    //初始化条码类型
    function initTypes() {
        $.get('/Group/GetGroupTypes',
            function (data) {
                $('.edit-type-select').select2({
                    data: data,
                    allowClear: false
                });
                var type = $('#DefaultValue').attr('data-type');
                $('.edit-type-select').val(type).trigger("change");
            });
    }
})();