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

        //初始化table
        initTable();

        //初始化zTree
        zTreeObj = $.fn.zTree.init($("#departmentTree"), setting);
        
    });

    //zTree节点单击回调函数
    function zTreeBeforeClick(treeId, treeNode, clickFlag) {
        alert(treeNode.id + "-" + treeNode.name);
    }


    //初始化table
    function initTable() {
        table.bootstrapTable({
            url: "/Department/GetListForTable",
            method: "get",
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            //toolbar: "#toolbar",
            pagination: true,
            clickToSelect: true,
            sidePagination: "server",
            //search: true,
            //showRefresh: true,
            //showExport: true,
            queryParams: function (params) {
                return {
                    parentId: parentId,
                    limit: params.limit,
                    offset: params.offset
                };
            },
            columns: [
                {
                    field: "Id",
                    title: "标识"
                },
                {
                    field: "Name",
                    title: "名称"
                }, {
                    field: "FullName",
                    title: "全称"
                }, {
                    field: "Code",
                    title: "代码"
                }, {
                    field: "SerialNumber",
                    title: "序号"
                }, {
                    field: "Id",
                    title: "操作",
                    formatter: function (value) {
                        var e = '<a class="btn btn-info btn-xs" href="javascript:Edit(' +
                            value +
                            ')"><i class="fa fa-edit"></i>修改</a>&nbsp;';
                        var d = '<a class="btn btn-danger btn-xs" href="javascript:Delete(' +
                            value +
                            ');"><i class="fa fa-trash"></i>删除</a> ';
                        return e + d;
                    }
                }
            ]
        });
    }

})();