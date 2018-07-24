var CreatedOKLodop7766 = null;

//====判断是否需要安装CLodop云打印服务器:====
function needCLodop() {
    try {
        var ua = navigator.userAgent;
        if (ua.match(/Windows\sPhone/i) != null) return true;
        if (ua.match(/iPhone|iPod/i) != null) return true;
        if (ua.match(/Android/i) != null) return true;
        if (ua.match(/Edge\D?\d+/i) != null) return true;

        var verTrident = ua.match(/Trident\D?\d+/i);
        var verIe = ua.match(/MSIE\D?\d+/i);
        var verOpr = ua.match(/OPR\D?\d+/i);
        var verFf = ua.match(/Firefox\D?\d+/i);
        var x64 = ua.match(/x64/i);
        if ((verTrident == null) && (verIe == null) && (x64 !== null))
            return true; else
            if (verFf !== null) {
                verFf = verFf[0].match(/\d+/);
                if ((verFf[0] >= 42) || (x64 !== null)) return true;
            } else
                if (verOpr !== null) {
                    verOpr = verOpr[0].match(/\d+/);
                    if (verOpr[0] >= 32) return true;
                } else
                    if ((verTrident == null) && (verIe == null)) {
                        var verChrome = ua.match(/Chrome\D?\d+/i);
                        if (verChrome !== null) {
                            verChrome = verChrome[0].match(/\d+/);
                            if (verChrome[0] >= 42) return true;
                        };
                    };
        return false;
    } catch (err) { return true; }
};

//====页面引用CLodop云打印必须的JS文件：====
if (needCLodop()) {
    var head = document.head || document.getElementsByTagName("head")[0] || document.documentElement;
    var oscript = document.createElement("script");
    oscript.src = "http://localhost:8000/CLodopfuncs.js?priority=1";
    head.insertBefore(oscript, head.firstChild);

    //引用双端口(8000和18000）避免其中某个被占用：
    oscript = document.createElement("script");
    oscript.src = "http://localhost:18000/CLodopfuncs.js?priority=0";
    head.insertBefore(oscript, head.firstChild);
};

//====获取LODOP对象的主过程：====
function getLodop(oObject, oEmbed) {
    var strHtmInstall = "打印控件未安装!<a href='/down/install_lodop32.rar' target='_blank'>点击这里执行安装</a>,安装后请刷新页面或重新进入。";
    var strHtmUpdate = "打印控件需要升级!<a href='/down/install_lodop32.rar' target='_blank'>点击这里执行升级</a>,升级后请重新进入。";
    var strHtm64Install = "打印控件未安装!<a href='/down/install_lodop64.rar' target='_blank'>点击这里执行安装</a>,安装后请刷新页面或重新进入。";
    var strHtm64Update = "打印控件需要升级!<a href='/down/install_lodop64.rar' target='_blank'>点击这里执行升级</a>,升级后请重新进入。";
    var strHtmFireFox = "</br>（注意：如曾安装过Lodop旧版附件npActiveXPLugin,请在【工具】->【附加组件】->【扩展】中先卸它）";
    var strHtmChrome = "<br>(如果此前正常，仅因浏览器升级或重安装而出问题，需重新执行以上安装）";
    var strCLodopInstall = "CLodop云打印服务(localhost本地)未安装启动!<a href='/down/CLodop_Setup_for_Win32NT.rar' target='_blank'>点击这里执行安装</a>,安装后请刷新页面。";
    var strCLodopUpdate = "CLodop云打印服务需升级!<a href='/down/CLodop_Setup_for_Win32NT.rar' target='_blank'>点击这里执行升级</a>,升级后请刷新页面。";
    var lodop;
    var err;
    try {
        var isIe = (navigator.userAgent.indexOf('MSIE') >= 0) || (navigator.userAgent.indexOf('Trident') >= 0);
        var is64Ie;
        if (needCLodop()) {
            try { lodop = window.getCLodop(); } catch (err) { };
            if (!lodop && document.readyState !== "complete") { alert("C-Lodop没准备好，请稍后再试！"); return lodop; };
            if (!lodop) {
                if (isIe) showTips(strCLodopInstall); else
                    showTips(strCLodopInstall);
                return lodop;
            } else {

                if (window.CLODOP.CVERSION < "2.1.6.3") {
                    if (isIe) showTips(strCLodopUpdate); else
                        showTips(strCLodopUpdate);
                };
                if (oEmbed && oEmbed.parentNode) oEmbed.parentNode.removeChild(oEmbed);
                if (oObject && oObject.parentNode) oObject.parentNode.removeChild(oObject);
            };
        } else {
            is64Ie = isIe && (navigator.userAgent.indexOf('x64') >= 0);
            //=====如果页面有Lodop就直接使用，没有则新建:==========
            if (oObject != undefined || oEmbed != undefined) {
                if (isIe) lodop = oObject; else lodop = oEmbed;
            } else if (CreatedOKLodop7766 == null) {
                lodop = document.createElement("object");
                lodop.setAttribute("width", 0);
                lodop.setAttribute("height", 0);
                lodop.setAttribute("style", "position:absolute;left:0;top:-100px;width:0;height:0;");
                if (isIe) lodop.setAttribute("classid", "clsid:2105C259-1E0C-4534-8141-A753534CB4CA");
                else lodop.setAttribute("type", "application/x-print-lodop");
                document.documentElement.appendChild(lodop);
                CreatedOKLodop7766 = lodop;
            } else lodop = CreatedOKLodop7766;
            //=====Lodop插件未安装时提示下载地址:==========
            if ((lodop == null) || (typeof (lodop.VERSION) == "undefined")) {
                if (navigator.userAgent.indexOf('Chrome') >= 0)
                    document.documentElement.innerHTML = strHtmChrome + document.documentElement.innerHTML;
                if (navigator.userAgent.indexOf('Firefox') >= 0)
                    document.documentElement.innerHTML = strHtmFireFox + document.documentElement.innerHTML;
                if (is64Ie) showTips(strHtm64Install); else
                    if (isIe) showTips(strHtmInstall); else
                        document.documentElement.innerHTML = strHtmInstall + document.documentElement.innerHTML;
                return lodop;
            };
        };
        if (lodop.VERSION < "6.2.1.8") {
            if (!needCLodop()) {
                if (is64Ie) showTips(strHtm64Update); else
                    if (isIe) showTips(strHtmUpdate); else
                        document.documentElement.innerHTML = strHtmUpdate + document.documentElement.innerHTML;
            };
            return lodop;
        };
        //===如下空白位置适合调用统一功能(如注册语句、语言选择等):===
        lodop.SET_LICENSES("北京远光通联科技有限公司", "653716277737475919278901905623", "", "");
        //===========================================================
        return lodop;
    } catch (err) { alert("getLodop出错:" + err); };
    return lodop;
};

function showTips(html) {
    var fullHtml = '<div class="callout callout-warning"><h4> 警告!</h4><p>' + html + '</p></div >';
    $(".content-wrapper .content").prepend(fullHtml);
}
