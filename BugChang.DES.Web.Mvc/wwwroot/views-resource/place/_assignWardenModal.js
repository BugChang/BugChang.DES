(function () {
    $(function () {

        var placeId = $('#AssignWardenDefaultValue').attr('data-place-id');
        initWardens(placeId);

        $("#AssignWardenForm").submit(function (e) {
            e.preventDefault();
            var wardenIds = $(".warden-select").val();
            var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
            $.ajax({
                type: 'POST',
                async: false,
                cache: false,
                data: { placeId: placeId, wardenIds: wardenIds },
                headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
                url: "/Place/AssignWarden",
                success: function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#AssignWardenModal").modal("hide");
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                }
            });
        });
    });

    function initWardens(placeId) {
        $.get('/Place/GetWardens/' + placeId,
            function (data) {
                $('.warden-select').select2({
                    placeholder: "请选择交换场所管理员",
                    data: data,
                    allowClear: true
                });
            });
    }
})();