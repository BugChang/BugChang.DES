(function () {
    $(function () {

        initPlace();

        $("#BoxEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Box/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#BoxEditModal").modal("hide");
                        //刷新父页面
                        BoxIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    //初始化交换场所
    function initPlace() {
        $.get('/Box/GetPlaces',
            function (data) {
                $('.edit-place-select').select2({
                    data: data,
                    allowClear: false
                });
                var placeId = $('#DefaultValue').attr('data-place-id');
                $('.edit-place-select').val(placeId).trigger("change");
            });
    }
})();