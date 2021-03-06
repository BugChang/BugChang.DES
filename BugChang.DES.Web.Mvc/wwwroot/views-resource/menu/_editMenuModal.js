﻿(function () {
    $(function () {
        initSelect();
        $("#MenuEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Menu/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#MenuEditModal").modal("hide");
                        //刷新父页面
                        MenuIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    function initSelect() {
        $.get("/Menu/GetListForSelect",
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