(function () {
    $(function () {

        $('#place').delegate('a',
            'click',
            function () {
                var id = $(this).attr('data-id');
                if (id === "diy") {
                    loadPlaceDiy();
                }
                if (id === "day") {
                    loadPlaceDay();
                }
                if (id === "month") {
                    loadPlaceMonth();
                }
                if (id === "quarter") {
                    loadPlaceQuarter();
                }
                if (id === "year") {
                    loadPlaceYear();
                }
            });

        $('#department').delegate('a',
            'click',
            function () {
                var id = $(this).attr('data-id');
                if (id === "diy") {
                    loadDepartmentDiy();
                }
                if (id === "day") {
                    loadDepartmentDay();
                }
                if (id === "month") {
                    loadDepartmentMonth();
                }
                if (id === "quarter") {
                    loadDepartmentQuarter();
                }
                if (id === "year") {
                    loadDepartmentYear();
                }
            });
    });

    function loadPlaceYear() {
        var myDate = new Date();//获取系统当前时间
        var beginDate = dateFormat(myDate, "yyyy-01-01");
        myDate.setFullYear(myDate.getFullYear() + 1);
        var endDate = dateFormat(myDate, "yyyy-01-01");
        $('#placeYear .box-body').load('/Letter/StatisticsPlace/', { beginDate: beginDate, endDate: endDate });
    }

    function loadPlaceQuarter() {
        var myDate = new Date();//获取系统当前时间
        var month = myDate.getMonth();
        var beginDate = dateFormat(myDate, "yyyy-MM-dd");
        var endDate = dateFormat(myDate, "yyyy-MM-dd");
        if (month < 3) {
            beginDate = dateFormat(myDate, "yyyy-01-01");
            endDate = dateFormat(myDate, "yyyy-04-01");
        }
        if (month > 2 && month < 6) {
            beginDate = dateFormat(myDate, "yyyy-04-01");
            endDate = dateFormat(myDate, "yyyy-07-01");
        }
        if (month > 5 && month < 9) {
            beginDate = dateFormat(myDate, "yyyy-07-01");
            endDate = dateFormat(myDate, "yyyy-10-01");
        }
        if (month > 8) {
            beginDate = dateFormat(myDate, "yyyy-10-01");
            myDate.setFullYear(myDate.getFullYear() + 1);
            endDate = dateFormat(myDate, "yyyy-01-01");
        }

        $('#placeQuarter .box-body').load('/Letter/StatisticsPlace/', { beginDate: beginDate, endDate: endDate });
    }

    function loadPlaceMonth() {
        var myDate = new Date();//获取系统当前时间
        var beginDate = dateFormat(myDate, "yyyy-MM-01");
        myDate.setMonth(myDate.getMonth() + 1);
        var endDate = dateFormat(myDate, "yyyy-MM-01");
        $('#placeMonth .box-body').load('/Letter/StatisticsPlace/', { beginDate: beginDate, endDate: endDate });
    }

    function loadPlaceDay() {
        var myDate = new Date();//获取系统当前时间
        var beginDate = dateFormat(myDate, "yyyy-MM-dd");
        myDate.setDate(myDate.getDate() + 1);
        var endDate = dateFormat(myDate, "yyyy-MM-dd");
        $('#placeDay .box-body').load('/Letter/StatisticsPlace/', { beginDate: beginDate, endDate: endDate });
    }

    function loadPlaceDiy() {

    }

    function loadDepartmentYear() {
        var myDate = new Date();//获取系统当前时间
        var beginDate = dateFormat(myDate, "yyyy-01-01");
        myDate.setFullYear(myDate.getFullYear() + 1);
        var endDate = dateFormat(myDate, "yyyy-01-01");
        $('#departmentYear .box-body').load('/Letter/StatisticsDepartment/', { id: 0, beginDate: beginDate, endDate: endDate });
    }

    function loadDepartmentQuarter() {
        var myDate = new Date();//获取系统当前时间
        var month = myDate.getMonth();
        var beginDate = dateFormat(myDate, "yyyy-MM-dd");
        var endDate = dateFormat(myDate, "yyyy-MM-dd");
        if (month < 3) {
            beginDate = dateFormat(myDate, "yyyy-01-01");
            endDate = dateFormat(myDate, "yyyy-04-01");
        }
        if (month > 2 && month < 6) {
            beginDate = dateFormat(myDate, "yyyy-04-01");
            endDate = dateFormat(myDate, "yyyy-07-01");
        }
        if (month > 5 && month < 9) {
            beginDate = dateFormat(myDate, "yyyy-07-01");
            endDate = dateFormat(myDate, "yyyy-10-01");
        }
        if (month > 8) {
            beginDate = dateFormat(myDate, "yyyy-10-01");
            myDate.setFullYear(myDate.getFullYear() + 1);
            endDate = dateFormat(myDate, "yyyy-01-01");
        }
        $('#departmentQuarter .box-body').load('/Letter/StatisticsDepartment/', { id: 0, beginDate: beginDate, endDate: endDate });
    }

    function loadDepartmentMonth() {

        var myDate = new Date();//获取系统当前时间
        var beginDate = dateFormat(myDate, "yyyy-MM-01");
        myDate.setMonth(myDate.getMonth() + 1);
        var endDate = dateFormat(myDate, "yyyy-MM-01");
        $('#departmentMonth .box-body').load('/Letter/StatisticsDepartment/', { id: 0, beginDate: beginDate, endDate: endDate });
    }

    function loadDepartmentDay() {
        var myDate = new Date();//获取系统当前时间
        var beginDate = dateFormat(myDate, "yyyy-MM-dd");
        myDate.setDate(myDate.getDate() + 1);
        var endDate = dateFormat(myDate, "yyyy-MM-dd");
        $('#departmentDay .box-body').load('/Letter/StatisticsDepartment/', { id: 0, beginDate: beginDate, endDate: endDate });
    }

    function loadDepartmentDiy() {

    }


    function dateFormat(date, fmt) { //author: meizz   
        var o = {
            "M+": date.getMonth() + 1,                 //月份   
            "d+": date.getDate(),                    //日   
            "h+": date.getHours(),                   //小时   
            "m+": date.getMinutes(),                 //分   
            "s+": date.getSeconds(),                 //秒   
            "q+": Math.floor((date.getMonth() + 3) / 3), //季度   
            "S": date.getMilliseconds()             //毫秒   
        };
        if (/(y+)/.test(fmt))
            fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (o.hasOwnProperty(k))
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1,
                        (RegExp.$1.length === 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }
})();