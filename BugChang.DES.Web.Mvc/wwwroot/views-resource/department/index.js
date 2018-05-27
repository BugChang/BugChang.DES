(function () {
    var parentId = null;
    var table = $("#table");
    var zTreeObj;//zTree对象
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
                type: 'post', // 提交方式 get/post
                url: '/Department/Edit', // 需要提交的 url
                data: data,
                success: function (d) { // data 保存提交后返回的数据，一般为 json 数据
                    // 此处可对 data 作相关处理
                    alert('提交成功！');
                }
            });
        });

    });

    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode, clickFlag) {
        parentId = treeNode.id;
        table.DataTable().ajax.reload();
    }


    //初始化table
    function initTable() {
        table.DataTable({
            "ordering": false,
            "processing": true,
            "serverSide": true,
            "autoWith": true,
            "ajax": {
                "url": "/Department/GetListForTable",
                "data": function (d) {
                    //添加额外的参数传给服务器
                    d.parentId = parentId;
                }
            },
            "stateSave": true,
            "columns": [
                { "data": "id" },
                { "data": "name" },
                { "data": "fullName" },
                { "data": "code" }
            ],
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
})();