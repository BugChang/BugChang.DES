(function () {
    $(function () {

        initUsers();

        $("#CardEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Card/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#CardEditModal").modal("hide");
                        //刷新父页面
                        CardIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });
    //初始化用户下拉列表
    function initUsers() {
        $.get('/Card/GetUsers',
            function (data) {
                $('.edit-user-select').select2({
                    data: data,
                    allowClear: false
                });
                var userId = $("#DefaultValue").attr("data-user-id");
                $(".edit-user-select").val(userId).trigger("change");
            });
    }
})();