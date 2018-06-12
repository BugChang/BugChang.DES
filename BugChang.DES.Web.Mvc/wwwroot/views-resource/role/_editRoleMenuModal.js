(function () {
    var zTreeObj;
    // zTree 参数配置
    var setting = {
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId",
                rootPId: null
            }
        },
        check: {
            enable: true
        }
    };
    $(function () {
        //初始化zTree
        initTree();
    });

    function initTree() {
        $.get('/Role/GetTreeForRoleMenu',
            function (nodes) {
                zTreeObj = $.fn.zTree.init($('#menuTree'), setting, nodes);
            });
    }
})();