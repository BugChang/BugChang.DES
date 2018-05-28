(function () {
    var currentNode = null;
    var table;
    var zTreeObj;

    var setting = {// zTree 参数配置
        async: {
            enable: true,
            url: "/Department/GetTreeData",
            autoParam: ["id=parentId"],
            type: "get"
        },
        callback: {
            beforeClick: zTreeBeforeClick
        }
    };

    $(function () {

        //初始化zTree
        zTreeObj = $.fn.zTree.init($("#departmentTree"), setting);

        //初始化table
        initTable();

        //初始化select2选择框
        initSelect();

        $("#DepartmentCreateForm").submit(function (e) {
            e.preventDefault();
            var data = $(this).serialize();
            $(this).ajaxSubmit({
                type: "post",
                url: "/Department/Edit",
                data: data,
                success: function (result) {
                    if (result.success) {
                        resetForm();
                        //关闭模态
                        $("#DepartmentCreateModal").modal("hide");
                        //刷新表格
                        table.ajax.reload();
                        //刷新机构树
                        zTreeObj.reAsyncChildNodes(currentNode, "refresh");
                    } else {
                        alert(result.message);
                    }

                }
            });
        });

        $("table").delegate(".view-department", "click", function () {
            var departmentId = $(this).attr("data-department-id");
            viewDepartment(departmentId);
        });

        $("table").delegate(".edit-department", "click", function () {
            var departmentId = $(this).attr("data-department-id");
            editDepartment(departmentId);
        });

        $("table").delegate(".delete-department", "click", function () {
            var departmentId = $(this).attr("data-department-id");
            deleteDepartment(departmentId);
        });

    });

    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode) {
        currentNode = treeNode;
        table.ajax.reload();
        $(".select2").val(treeNode.id).trigger("change");
    }


    //初始化table
    function initTable() {
        table = $("#table").DataTable({
            ordering: false,
            processing: true,
            serverSide: true,
            autoWith: true,
            ajax: {
                url: "/Department/GetListForTable",
                data: function (para) {
                    //添加额外的参数传给服务器
                    para.parentId = currentNode === null ? null : currentNode.id;
                }
            },
            stateSave: true,
            columns: [
                {
                    data: "id",
                    title: "主键"

                },
                {
                    data: "name",
                    title: "名称"
                },
                {
                    data: "fullName",
                    title: "全称"
                },
                {
                    data: "code",
                    title: "代码"
                },
                {
                    data: null,
                    title: "操作"
                }
            ],
            columnDefs: [{
                targets: 4,
                render: function (data, type, row) {
                    var strHtml =
                        '<button class="btn btn-info btn-xs view-department" data-department-id=' + row.id + '>查看</button>&nbsp;' +
                        '<button class="btn btn-warning btn-xs edit-department" data-department-id=' + row.id + '>修改</button>&nbsp;' +
                        '<button class="btn btn-danger btn-xs delete-department" data-department-id=' + row.id + '>删除</button>';
                    return strHtml;
                }
            }],
            language: {
                url: '../../lib/datatables/language/chinese.json'
            }
        });
    }

    function initSelect() {
        $.get("/Department/GetListForSelect",
            function (data) {
                $(".select2").select2({
                    data: data,
                    placeholder: "请选择上级机构",
                    allowClear: true
                });
            });
    }

    function viewDepartment(id) {
        alert("查看" + id);
    }

    function editDepartment(id) {
        $("#DepartmentEditModal .modal-content").load("/Department/EditDepartmentModal/" + id);
        $("#DepartmentEditModal").modal('show');
    }

    function deleteDepartment(id) {
        alert("删除" + id);
    }

    //清空表单
    function resetForm() {
        $("#DepartmentCreateForm").resetForm();
        $(".select2").val(currentNode.id).trigger("change");
    }
})();

