(function () {
    $(function () {
        initDepartmentSelect();
        initParentPlaceSelect();
        $("#PlaceEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Exchange/PlaceEdit',
                data,
                function (result) {
                    if (result.success) {
                        //关闭模态
                        $("#PlaceEditModal").modal("hide");
                        //刷新父页面
                        ExchangePlace.refresh();
                        window.toastr.success('操作成功');
                    } else {
                        window.toastr.error(result.message);
                    }
                });
        });
    });

    //初始化机构列表
    function initDepartmentSelect() {
        $.get('/Exchange/GetDepartmentsForSelect',
            function (data) {
                $('.department-select').select2({
                    data: data,
                    placeholder: '请选择机构',
                    allowClear: false
                });
                $('.department-select').val($('.department-select').val()).trigger("change");
            });
    }

    //初始化上级交换场所列表
    function initParentPlaceSelect() {
        $.get('/Exchange/GetPlacesForSelect',
            function (data) {
                $('.parent-select').select2({
                    data: data,
                    placeholder: '请选择上级交换场所',
                    allowClear: false
                });
                console.log($('.parent-select').val());
                console.log($('#ParentId').val());
                $(".parent-select").val($(".parent-select").val()).trigger("change");
            });
    }
})();