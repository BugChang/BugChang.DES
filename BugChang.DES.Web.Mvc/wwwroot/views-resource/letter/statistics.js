(function () {
    $(function () {

        $('#place').delegate('a',
            'click',
            function () {
                var id = $(this).attr('data-id');
                if (id!=="diy") {
                    alert(id);
                }
               
            });
    });
    
})();