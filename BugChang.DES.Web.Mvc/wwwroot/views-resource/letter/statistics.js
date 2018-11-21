(function () {
    $('.search-time').datetimepicker({
        format: 'yyyy-mm-dd',
        language: 'zh-CN',
        autoclose: true,
        minView: 2
    });
    $("form").submit(function (e) {
        $("#LoadingModal").modal('show');
    });
   
})();