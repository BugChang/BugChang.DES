(function () {
    $(function () {
        $("#ChangePasswordForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Account/ChangePassword',
                data,
                function (result) {
                    if (result.success) {
                        window.swal(
                            '密码修改成功，请重新登录系统！',
                            '',
                            'success'
                        ).then(() => {
                            location.href = "/Account/Logout";
                        });
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });
})();