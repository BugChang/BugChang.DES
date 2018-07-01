(function () {
    $(function () {
        initDepartmentSelect();
        initParentPlaceSelect();
        $("#PlaceEditForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $.post('/Place/Edit',
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
        $.get('/Place/GetDepartmentsForSelect',
            function (data) {
                $('.edit-department-select').select2({
                    data: data,
                    placeholder: '请选择机构',
                    allowClear: false
                });
                var departmentId = $('#DefaultValue').attr('data-department-id');
                $('.edit-department-select').val(departmentId).trigger("change");
            });
    }

    //初始化上级交换场所列表
    function initParentPlaceSelect() {
        $.get('/Place/GetPlacesForSelect',
            function (data) {
                $('.edit-parent-select').select2({
                    data: data,
                    placeholder: '请选择上级交换场所',
                    allowClear: false
                });
                var parentId = $('#DefaultValue').attr('data-parent-id');
                $(".edit-parent-select").val(parentId).trigger("change");
            });
    }
})();