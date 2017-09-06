//request define
//get cached location for the client ip address, 
amplify.request.define("getCachedLocation", "ajax", {
    url: "/api/location/cachedlocation",
    dataType: "json",
    type: "GET"
});

amplify.request.define("refreshUserCity", "ajax", {
    url: "/api/location/refreshusercity",
    type: "post"
});

amplify.request.define("updateCachedLocation", "ajax", {
    url: "/api/location/cachedlocation",
    type: "post"
});

amplify.request.define("DistanceBetween", "ajax", {
    url: "/api/Location/DistanceBetween?loc1={loc1}&loc2={loc2}",
    type: "get"
});

amplify.request.define("GetAndCacheIpLocation", "ajax", {
    url: "/api/Location/GetAndCacheIpLocation",
    type: "get"
});

amplify.request.define("GetIpLocation", "ajax", {
    url: "/api/Location/GetIpLocation",
    type: "get"
});

amplify.request.define("changePassword", "ajax", {
    url: "/api/account/changePassword",
    type: "post"
});

amplify.request.define("ToggleLike", "ajax", {
    url: "/api/lrf/togglelike",
    type: "post"
});

amplify.request.define("ToggleFollow", "ajax", {
    url: "/api/account/following/{id}",
    type: "post"
});

amplify.request.define("ToggleApprove", "ajax", {
    url: "/api/Editor/ToggleApproveHouse/{id}",
    type: "post"
});

amplify.request.define("ToggleEssence", "ajax", {
    url: "/api/Editor/ToggleEssenceCheckIn/{id}",
    type: "post"
});

amplify.request.define("ToggleRole", "ajax", {
    url: "/api/account/ToggleRole",
    data: { id: "", role:"" },
    type: "post"
});

amplify.request.define("MyHouses", "ajax", {
    url: "/api/content/MyHouses",
    data: {id : ""},
    type: "get"
});

amplify.request.define("MyCheckins", "ajax", {
    url: "/api/content/MyCheckins",
    data: { id: "" },
    type: "get"
});

amplify.request.define("QRCode", "ajax", {
    url: "/api/content/qrcode",
    data: { url: "" },
    type: "get"
});

amplify.request.define("deleteFeedBack", "ajax", {
    url: "/api/feedback/deleteOne?id={id}",
    type: "delete"
});

jQuery.AutoComplete = function (selector) {
    var elt = $(selector);
    var autoComplete, autoLi;
    var strHtml = [];
    strHtml.push('<div class="AutoComplete" id="AutoComplete">');
    strHtml.push('    <ul class="AutoComplete_ul">');
    strHtml.push('        <li class="AutoComplete_title">请选择邮箱后缀</li>');
    strHtml.push('        <li hz="@qq.com"></li>');
    strHtml.push('        <li hz="@163.com"></li>');
    strHtml.push('        <li hz="@hust.edu.cn"></li>');
    strHtml.push('        <li hz="@126.com"></li>');
    strHtml.push('        <li hz="@sohu.com"></li>');
    strHtml.push('        <li hz="@sina.com"></li>');
    strHtml.push('        <li hz="@gmail.com"></li>');
    strHtml.push('    </ul>');
    strHtml.push('</div>');
    $('body').append(strHtml.join(''));
    $('#AutoComplete').css('width', elt.css('width'));

    autoComplete = $('#AutoComplete');
    autoComplete.data('elt', elt);
    autoLi = autoComplete.find('li:not(.AutoComplete_title)');
    autoLi.mouseover(function () {
        $(this).siblings().filter('.hover').removeClass('hover');
        $(this).addClass('hover');
    }).mouseout(function () {
        $(this).removeClass('hover');
    }).mousedown(function () {
        autoComplete.data('elt').val($(this).text()).change();
        autoComplete.hide();
    });

    elt.keyup(function (e) {
        if (/13|38|40|116/.test(e.keyCode) || this.value == '') {
            return false;
        }
        var name = this.value;
        if (name.indexOf('@') == -1) {
            autoComplete.hide();
            return false;
        }
        autoLi.each(function () {
            this.innerHTML = name.replace(/\@+.*/, '') + $(this).attr('hz');
            if (this.innerHTML.indexOf(name) >= 0) {
                $(this).show();
            } else {
                $(this).hide();
            }
        }).filter('.hover').removeClass('hover');
        autoComplete.show().css({
            left: $(this).offset().left,
            top: $(this).offset().top + $(this).outerHeight(true) - 1,
            position: 'absolute',
            zIndex: '99999'
        });
        if (autoLi.filter(':visible').length == 0) {
            autoComplete.hide();
        } else {
            autoLi.filter(':visible').eq(0).addClass('hover');
        }
    }).keydown(function (e) {
        if (e.keyCode == 38) { //上
            autoLi.filter('.hover').prev().not('.AutoComplete_title').addClass('hover').next().removeClass('hover');
        } else if (e.keyCode == 40) { //下
            autoLi.filter('.hover').next().addClass('hover').prev().removeClass('hover');
        } else if (e.keyCode == 13) { //Enter
            autoLi.filter('.hover').mousedown();
            e.preventDefault();    //如有表单，阻止表单提交
        }
    }).focus(function () {
        autoComplete.data('elt', $(this));
    }).blur(function () {
        autoComplete.hide();
    });
};

function showLogin() {
    $("#Login").addClass("active");
    $("#Register").removeClass("active");
    $(".loginForm>.login").show();
    $(".loginForm>.register").hide();
}
function showRegister() {
    $("#Login").removeClass("active");
    $("#Register").addClass("active");
    $(".loginForm>.login").hide();
    $(".loginForm>.register").show();
}
function showEmailLogin() {
    $("#EmailLogin").addClass("active");
    $("#QuickLogin").removeClass("active");
    $(".loginForm>.emailLogin").show();
    $(".loginForm>.quickLogin").hide();
}
function showQuickLogin() {
    $("#EmailLogin").removeClass("active");
    $("#QuickLogin").addClass("active");
    $(".loginForm>.emailLogin").hide();
    $(".loginForm>.quickLogin").show();
}

function checkLength(obj, length, message) {
    if (!length) {
        var length = 200;
    }
    if(!message) {
        message = "";
    }
    else {
        message = "(" + message + ")";
    }
    var value = obj.value;
    if (value.length >= length) {
        obj.value = obj.value.substr(0, length);
        $(obj).siblings("span.validNum").text(message);
    } else {
        $(obj).siblings("span.validNum").text("（还可以输入" + (length - value.length) + "字）");
    }
};

function checkMe(elem) {
    var checkbox = $(elem).siblings("input[type=checkbox]");
    checkbox.prop("checked", !checkbox.is(":checked"));
}

function radioMe(elem) {
    var radiobox = $(elem).prev("input[type=radio]");
    radiobox.prop("checked", true);
}

function is_weixin() {
    var ua = navigator.userAgent.toLowerCase();
    if(ua.match(/MicroMessenger/i) == "micromessenger")
    {
        return true;
    }
    else
    {
        return false;
    }
}
//获取url中query的对象
function getQueryJson() {
    var ret = {};
    var query = decodeURI(window.location.search.substr(1)).split("&");
    for (var i = 0; i <query.length; i++)
    {
        var j = query[i].indexOf("=");
        ret[query[i].substring(0, j)] = query[i].substring(j+1);
    }
    return ret;
}
//将对象序列化为url中的query字符串
function jsonToSearch(o) {
    var arr = [];
    for (var i in o) {
        if (i != "") {
            arr.push(i + "=" + o[i]);
        }
    } 
    return encodeURI(arr.join('&'));
}

//HTML转义
function HTMLEncode(html)
{
    var temp = document.createElement ("div");
    (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
    var output = temp.innerHTML;
    temp = null;
    return output;
}
//HTML反转义
function HTMLDecode(text)
{
    var temp = document.createElement("div");
    temp.innerHTML = text;
    var output = temp.innerText || temp.textContent;
    temp = null;
    return output;
}

function getLocation(success) {
    var useCache;
    //display loading gif
    $('#loading-more').load("/home/loadingstart");

    //try to get from cache
    amplify.request({
        resourceId: "getCachedLocation",
        success: function(data) {
            //test if the result is null
            if (data != null) {
                console.log("using cached: " + data.Longitude + ";" + data.Latitude);
                var point = {
                    country: data.Country,
                    province: data.Province,
                    city: data.City,
                    coords: {
                        longitude: data.Longitude,
                        latitude: data.Latitude
                    }
                };
                success(point);
                $('#loading-more').empty();
                $('.page').show();
            } else {
                //alert("new location");
                //get real location and update the cahce
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function(data) {
                        console.log("browser get location successed");
                        getLocationSuccess(success, data);
                        $('#loading-more').empty();
                        $('.page').show();
                    },
                    function() {    // time out
                        console.log("browser location time out, try ip");
                        //ip location before use fake location
                        amplify.request({
                            resourceId: "GetAndCacheIpLocation",
                            success: function (data) {
                                console.log("ip location success ip: "+data.IpAddress);
                                var point = {   //locationdto from ip location
                                    country: data.Country,
                                    province: data.Province,
                                    city: data.City,
                                    coords: {
                                        longitude: data.Longitude,
                                        latitude: data.Latitude
                                    }
                                };
                                getLocationSuccess(success, point);
                                $('#loading-more').empty();
                                $('.page').show();
                            },
                            error: function () {//the ip location nork working
                                getFakeLocation(success);
                                $('#loading-more').empty();
                                $('.page').show();

                                alertModal("无法获得您的地理位置，用华科毛爷爷广场代替吧",3000);
                            }
                        });
                    },
                    {
                        maximumAge: 600000, timeout: 30000
                    });

                } else {//geolocation is not suported
                    getFakeLocation(success);
                    $('#loading-more').empty();
                    $('.page').show();
                }

            }
        }
    });
}

//if the browers succesfully finished geoLocation function
//call this to do thing with the location point
//this will transform wsg to gcj, we assume the browser location and the ip location is wsg
function getLocationSuccess(success,point) {
    var point2 = Wsg2Mars(point);
    console.log("new location find");
    success(point2);
    $('#loading-more').empty();
    $('.page').show();
    //save to cache
    amplify.request("updateCachedLocation",
        {
            Longitude: point2.coords.longitude,
            Latitude: point2.coords.latitude
        }
    );
}


//wgs-84 to gcj-02
function Wsg2Mars(point) {
    var gcjloc = transformFromWGSToGCJ(point.coords.longitude, point.coords.latitude);
    var point2 = {
        coords: {
            longitude: gcjloc.lng,
            latitude: gcjloc.lat
        }
    };
    return point2;
}


//get a fake location 
function getFakeLocation(success) {
    var point = {
        coords: {
            longitude: 114.413429,
            latitude: 30.509033
        }
    };
    success(point);
}

//updata the distance that from the given point to a point in the distancee label
function updateDistance(distanceLabel, point) {
    //target location in a data tag "data-location"
    $("." + distanceLabel).each(function () {
        var self = $(this);
        var point2 = self.attr("data-location");
        amplify.request({
            resourceId: "DistanceBetween",
            data: {
                loc1: point,
                loc2: point2
            },
            success: function (data) {
                self.text(data);
            }
        });
    });
    
}

function alertModal(content,seconds)
{
    if (seconds == undefined)
    {
        seconds = 1000;
    }
    $('#alertModal').modal('show');
    $('#alertModal p').text(content);
    $('#alertModal .countDown').text(seconds/1000);
    setTimeout(function () { $('#alertModal').modal('hide'); }, seconds);
}

function toggleContact(elem)
{
    $(elem).children(".glyphicon").toggleClass("glyphicon-chevron-down").toggleClass("glyphicon-chevron-up");
    $(elem).siblings(".frame").slideToggle("slow");
}

//the like is partial view you cannot use jquery there,
//so let it call this
//todo wan: add favorite and rate, not in a hurry, save it for later
function setupLike(likeId, UserClient) {
    var elems = $('.my-heart.' + likeId);
    //request coresponde api
    amplify.request({
        resourceId: "ToggleLike",
        data: {
            id:likeId
        },
        success: function (data) {
            elems.children(".glyphicon").toggleClass("glyphicon-heart").toggleClass("glyphicon-heart-empty");
            if (elems.children(".glyphicon").hasClass("glyphicon-heart"))
            {
                elems.attr("title", "撤销点赞");
            }
            else
            {
                elems.attr("title", "点赞");
            }
            elems.children(".LrfTimes").text(data.LrfCount);
        },
        error: function (data) {//the ip location nork working
            if (UserClient == "Browser") {
                $("#loginModal").modal("show");
            }
            console.log("require log in");
        }
    });
}

function toggleFollow(elem, followeeId, UserClient) {
    //request coresponde api
    var element = $("." + followeeId).find(".my-eye");
    amplify.request({
        resourceId: "ToggleFollow",
        data: {
            id: followeeId
        },
        success: function (data) {
            $(element).children(".glyphicon").toggleClass("glyphicon-eye-open").toggleClass("glyphicon-eye-close");
            $(element).attr("data-original-title", data == true ? "已关注" : "点击关注");
        },
        error: function () {
            if (UserClient == "Browser") {
                $("#loginModal").modal("show");
            }
            console.log("require log in");
        }
    });

}

function toggleApprove(elem, houseId, UserClient) {
    amplify.request({
        resourceId: "ToggleApprove",
        data: {
            id: houseId
        },
        success: function () {
            $(elem).children(".glyphicon").toggleClass("glyphicon-star-empty").toggleClass("glyphicon-star");
            $("img.approve").toggleClass("hidden");
        },
        error: function () {
            if (UserClient == "Browser") {
                $("#loginModal").modal("show");
            }
            console.log("require log in");
        }
    });
}

function toggleEssence(elem, checkinId, UserClient) {
    amplify.request({
        resourceId: "ToggleEssence",
        data: {
            id: checkinId
        },
        success: function () {
            $(elem).children(".glyphicon").toggleClass("glyphicon-star-empty").toggleClass("glyphicon-star");
            $(elem).closest(".media-body").find("img.essence").toggleClass("hidden");
        },
        error: function () {
            if (UserClient == "Browser") {
                $("#loginModal").modal("show");
            }
            console.log("require log in");
        }
    });
}

function toggleRole(elem, userId, UserClient) {
    $('#confirmModal').modal('show');
    if ($(elem).hasClass("true"))
    {
        var content = "确定【取消】改用户的角色【" + $(elem).data("title") + "】吗？";
    }
    else
    {
        var content = "确定为该用户【添加】角色【" + $(elem).data("title") + "】吗？";
    }
    $('#confirmModal p').text(content);
    $('#confirmModal button.btn-primary').off().one("click", function () {
        amplify.request({
            resourceId: "ToggleRole",
            data: {
                id: userId,
                role: $(elem).attr("id")
            },
            success: function () {
                $(elem).toggleClass("true").toggleClass("false");
                location.reload();
            },
            error: function () {
                if (UserClient == "Browser") {
                    $("#loginModal").modal("show");
                }
                console.log("require log in");
            }
        });
        $('#confirmModal').modal("hide");
    });
}

function deleteParent(elem)
{
    $(elem).parent().remove();
}

function deleteFeedBack(feedBackId, UserClient)
{
    console.log(feedBackId);
    $('#confirmModal').modal('show');
    $('#confirmModal p').text("确定删除此条用户反馈吗？");
    $('#confirmModal button.btn-primary').off().one("click", function () {
        amplify.request({
            resourceId: "deleteFeedBack",
            data: {
                id: feedBackId,
            },
            success: function () {
                $("li." + feedBackId).remove();
                //location.reload();
            },
            error: function () {
                if (UserClient == "Browser") {
                    $("#loginModal").modal("show");
                }
                console.log("require log in");
            }
        });
        $('#confirmModal').modal("hide");
    });
}

(function ($) {
    $.extend($.fn, {
        ///<summary>
        /// apply a Terrance multiple file upload UI
        ///</summary>
        uploadFiles: function (setting) {
            var ps = $.extend({
                //content holder(Object || css Selector)
                renderTo: $(document.body),
                //whether user can upload a file
                enable: true,
                //initial files
                initFiles: function () { },
                //size of plug in
                size: { width: 200, height: 50 },
                //name of images
                fileName: 'file',
                //fired when a file uploaded
                onComplete: function () { },
                //fired when a file deleted
                onDelete: function () { }
            }, setting);

            ps.renderTo = (typeof ps.renderTo == 'string' ? $(ps.renderTo) : ps.renderTo);

            var oneFile = $('<div class="oneFileContainer">'
                + '<img class="oneFileImg" name="' + ps.fileName + '[]"/>'
                +'<b class="oneFileDelete">X</b>'
                +'</div>');
        },
        ///<summary>
        /// set slider value
        ///</summary>
        ///<param name="v">percentage, must be a Float variable between 0 and 1</param>
        ///<param name="v">callback Function</param>
        setSliderValue: function (v, callback) {
            try {
                //validate
                if (typeof v == 'undefined' || v < 0 || v > 1) {
                    throw new Error('\'v\' must be a Float variable between 0 and 1.');
                }

                var s = this;

                //validate 
                if (typeof s == 'undefined' ||
                    typeof s.data == 'undefined' ||
                        typeof s.data.bar == 'undefined') {
                    throw new Error('You bound the method to an object that is not a slider!');
                }

                $sliderProcess(s, s.data.completed, v * s.data.bar.width());

                if (typeof callback != 'undefined') { callback(v); }
            }
            catch (e) {
                alertModal(e.message);
            }
        }
    });
})(jQuery);