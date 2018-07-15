(function () {
    $(function () {
        initSelect();
        initChannel();
        $("#DepartmentEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Department/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#DepartmentEditModal").modal("hide");
                        //刷新父页面
                        DepartmentIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    function initSelect() {
        $.get("/Department/GetListForSelect",
            function (data) {
                $(".edit-parent-select").select2({
                    data: data,
                    placeholder: "请选择上级机构",
                    allowClear: true
                });
                var parentId = $("#DefaultValue").attr("data-parent-id");
                $(".edit-parent-select").val(parentId).trigger("change");
            });
    }

    //初始化渠道
    function initChannel() {
        $.get('/Department/GetChannels',
            function (data) {
                $('.edit-channel-select').select2({
                    data: data,
                    placeholder: '请选择默认收件渠道',
                    allowClear: false
                });
                var channel = $("#DefaultValue").attr("data-channel");
                $(".edit-channel-select").val(channel).trigger("change");
            });
    }
})();