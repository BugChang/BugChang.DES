(function () {
    $(function () {
        initSelect();
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
                $(".select2").select2({
                    data: data,
                    placeholder: "请选择上级机构",
                    allowClear: true
                });
                $(".select2").val($(".select2").val()).trigger("change");
            });
    }
})();