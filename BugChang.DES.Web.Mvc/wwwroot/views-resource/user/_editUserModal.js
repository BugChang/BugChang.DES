(function () {
    $(function () {
        initSelect();
        $("#UserEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/User/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#UserEditModal").modal("hide");
                        //刷新父页面
                        UserIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    function initSelect() {
        $.get("/User/GetListForSelect",
            function (data) {
                $(".select2").select2({
                    data: data,
                    placeholder: "请选择机构",
                    allowClear: true
                });
                $(".select2").val($(".select2").val()).trigger("change");
            });
    }
})();