(function () {
    $(function () {

        var boxId = $('#AssignObjectDefaultValue').attr('data-box-id');
        initBoxObjects(boxId);

        $("#AssignObjectForm").submit(function (e) {
            e.preventDefault();
            var objectIds = $(".box-object-select").val();
            $.post('/Box/AssignObject',
                { boxId: boxId, objectIds: objectIds },
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#AssignObjectModal").modal("hide");
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    function initBoxObjects(boxId) {
        $.get('/Box/GetBoxObjects/' + boxId,
            function (data) {
                $('.box-object-select').select2({
                    placeholder: "请选择流转对象",
                    data: data,
                    allowClear: true
                });
            });
    }
})();