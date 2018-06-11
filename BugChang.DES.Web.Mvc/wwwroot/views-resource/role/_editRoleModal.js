(function () {
    $(function () {
        $("#RoleEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Role/Edit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#RoleEditModal").modal("hide");
                        //刷新父页面
                        RoleIndex.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });
})();