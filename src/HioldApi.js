/**
 * 海鸥封装的后端js库
 */
//是否为测试环境自动生成url
var testBaseUrl = "http://e.ytonidc.com:48533/";
var isDebug = false;

(function ($) {
    var HioldApi = /** @class */ (function () {
        function HioldApi() {
        }
        return HioldApi;
    }());
    //绑定原型对应方法
    //登录
    HioldApi.prototype.login = _login;
    //获取用户信息
    HioldApi.prototype.getUserInfo = _getUserInfo;
    //获取库存
    HioldApi.prototype.getPlayerStorage = _getPlayerStorage;
    //获取在售物品
    HioldApi.prototype.getPlayerOnSell = _getPlayerOnSell;
    //提取物品到游戏中
    HioldApi.prototype.dispachItemToGame = _dispachItemToGame;
    //获取玩家商店列表
    HioldApi.prototype.getUserShopList = _getUserShopList;
    //更新用户信息
    HioldApi.prototype.updateUserInfo = _updateUserInfo;




    //封装JQuery请求工具
    function AjaxRequestWithToken(apiPath, dataParam, token, resolve, reject) {
        $.ajax({
            type: "POST",
            url: isDebug ? testBaseUrl + apiPath : "" + apiPath,
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(dataParam),
            dataType: "json",
            headers: {
                'token': token,
            },
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
    }

    //封装JQuery请求工具
    function AjaxRequest(apiPath, dataParam, resolve, reject) {
        $.ajax({
            type: "POST",
            url: isDebug ? testBaseUrl + apiPath : "" + apiPath,
            contentType: "application/json;charset=UTF-8",
            data: JSON.stringify(dataParam),
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
    }

    /**
     * 用户登录
     * 请求参数示例 {"username":"用户名","password":"密码"}
     * @param {*} param
     */
    function _login(param) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequest("api/login", param, resolve, reject);
        });
    }

    /**
        * 获取用户信息
        * 请求参数示例 {"id":"用户名ID"}
        * @param {*} param
        */
    function _getUserInfo(param, token) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequestWithToken("api/getUserInfo", param, token, resolve, reject);
        });
    }

    /**
        * 获取用户库存信息
        * 请求参数示例 {"itemname":"物品名称","pageIndex":"分页页码","pageSize":"每页数量","class1":"物品类型1","class2":"物品类型2"}
        * class1的可用值:普通物品,特殊物品,积分商城,钻石商城
        * class2的可用值:Ammo/Weapons,Armor,Tools/Traps,Food/Cooking,Books,Chemicals,Mods,Resources,Science,Medical,Special Items,Decor/Miscellaneous,Tool/Weapon,clothing
        * @param {*} param
        */
    function _getPlayerStorage(param, token) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequestWithToken("api/getPlayerStorage", param, token, resolve, reject);
        });
    }

    /**
        * 获取用户在售物品
        * 请求参数示例 {"itemname":"物品名称","pageIndex":"分页页码","pageSize":"每页数量","class1":"物品类型1","class2":"物品类型2","id":"物品ID","sort":""}
        * class1的可用值:普通物品,特殊物品,积分商城,钻石商城
        * class2的可用值:Ammo/Weapons,Armor,Tools/Traps,Food/Cooking,Books,Chemicals,Mods,Resources,Science,Medical,Special Items,Decor/Miscellaneous,Tool/Weapon,clothing
        * sort的可用值:价格高到低,价格低到高
        * @param {*} param
        */
    function _getPlayerOnSell(param, token) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequestWithToken("api/getPlayerOnSell", param, token, resolve, reject);
        });
    }
    /**
        * 提取物品到游戏中
        * 请求参数示例 {"id":"物品id","count":"提取数量"}
        * @param {*} param
        */
    function _dispachItemToGame(param, token) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequestWithToken("api/dispachItemToGame", param, token, resolve, reject);
        });
    }

    /**
        * 获取玩家商店
        * 请求参数示例 {"name":"玩家名称","orderby":"排序方式","page":"页码","limit":"每页数量"}
        * @param {*} param
        */
    function _getUserShopList(param, token) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequestWithToken("api/getUserShopList", param, token, resolve, reject);
        });
    }

    /**
    * 更新玩家信息
    * 请求参数示例 {"shopname":"店铺名称","qq":"QQ号","avatar":"base64的图片"}
    * @param {*} param
    */
    function _updateUserInfo(param, token) {
        var _that = this;
        return new Promise(function (resolve, reject) {
            AjaxRequestWithToken("api/updateUserInfo", param, token, resolve, reject);
        });
    }


    //导出方法
    window.HioldApi = new HioldApi();
})(jQuery);
