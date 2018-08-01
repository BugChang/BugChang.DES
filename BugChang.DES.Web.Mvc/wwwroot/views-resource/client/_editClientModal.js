(function () {
    $(function () {

        initPlaces();

        $(".btn-device-code").click(function () {
            ClientIndex.getDeviceCode();
        });

        $("#ClientEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Client/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#ClientEditModal").modal("hide");
                        //刷新父页面
                        ClientIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    //初始化条码类型
    function initPlaces() {
        $.get('/Client/GetPlaces',
            function (data) {
                $('.place-select').select2({
                    data: data,
                    allowClear: false
                });
                var placeId = $('#DefaultValue').attr('data-place-id');
                $('.place-select').val(placeId).trigger("change");
            });
    }
})();