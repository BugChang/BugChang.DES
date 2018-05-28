(function () {
    $(function () {
        initSelect();
        $("#DepartmentEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $(this).ajaxSubmit({
                type: "post",
                url: "/Department/Edit",
                data: data,
                success: function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#DepartmentEditModal").modal("hide");
                    } else {
                        alert(result.message);
                    }
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