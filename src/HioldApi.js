/**
 * 海鸥封装的后端js库
 */
(function ($) {
    var HioldApi = /** @class */ (function () {
        function HioldApi() {
        }
        return HioldApi;
    }());
    /**
     * 用户登录API
     * @param {*} param
     */
    function _login(param) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: "POST",
                url: 'http://e.ytonidc.com:48533/api/login',
                contentType: "application/json;charset=UTF-8",
                data: JSON.stringify(param),
                dataType: "json",
                success: function (res) {
                    if (res.respCode && res.respCode == 1) {
                        resolve(res.data);
                    }
                    else {
                        reject(res.respMsg);
                    }
                    console.log(res);
                },
                error: function (result) {
                    reject(result);
                }
            });
        });
    }
    HioldApi.prototype.login = _login;
    //导出方法
    window.HioldApi = new HioldApi();
})(jQuery);
