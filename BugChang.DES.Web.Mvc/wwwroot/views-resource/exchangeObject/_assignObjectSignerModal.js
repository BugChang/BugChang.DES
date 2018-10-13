(function () {
    $(function () {

        var objectId = $('#AssignObjectSignerDefaultValue').attr('data-object-id');
        initObjectSigners(objectId);

        $("#AssignObjectSignerForm").submit(function (e) {
            e.preventDefault();
            var userIds = $(".object-signer-select").val();
            var token = $("input[name='BugChangFieldName']").val();//隐藏域的名称要改
            $.ajax({
                type: 'POST',
                async: false,
                cache: false,
                data: { objectId: objectId, userIds: userIds },
                headers:
                {
                    "BugChang-CSRF-HEADER": token //注意header要修改
                },
                url: "/ExchangeObject/AssignObjectSigner",
                success: function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#AssignObjectSignerModal").modal("hide");
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                }
            });
        });
    });

    function initObjectSigners(objectId) {
        $.get('/ExchangeObject/GetObjectSigners/' + objectId,
            function (data) {
                $('.object-signer-select').select2({
                    placeholder: "请选择流转对象签收人",
                    data: data,
                    allowClear: true
                });
            });
    }
})();