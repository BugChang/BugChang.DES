(function () {
    $(function () {
        $(document).ajaxStart(function () {
            Pace.restart();
        });
    });
})();