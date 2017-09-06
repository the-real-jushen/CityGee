//request define
//get cached location for the client ip address, 
amplify.request.define("getCachedLocation", "ajax", {
    url: "/api/location/cachedlocation",
    dataType: "json",
    type: "GET"
});

amplify.request.define("updateCachedLocation", "ajax", {
    url: "/api/location/cachedlocation",
    type: "post"
});

amplify.request.define("DistanceBetween", "ajax", {
    url: "/api/Location/DistanceBetween?loc1={loc1}&loc2={loc2}",
    type: "get"
});






function getLocation(success) {
    var useCache;
    //display loading gif
    $('#loading-img').load(
        "/home/loading"
        );

    //try to get from cache
    amplify.request({
        resourceId: "getCachedLocation",
        success: function(data) {
            //test if the result is null
            if (data != null) {
                //alert("using cached: " + data.Longitude + ";" + data.Latitude);
                var point = {
                    coords: {
                        longitude: data.Longitude,
                        latitude: data.Latitude
                    }
                };
                success(point);
                $('#loading-img').hide();
            } else {
                //alert("new location");
                //get real location and update the cahce
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function (data) {
                        console.log("new location find");
                        success(data);
                        $('#loading-img').hide();
                        //save to cache
                        amplify.request("updateCachedLocation",
                            {
                                Longitude: data.coords.longitude,
                                Latitude: data.coords.latitude
                            }
                        );
                    });

                } else {//geolocation is not suported
                    var point = {
                        coords: {
                            longitude: 0,
                            latitude: 0
                        }
                    };
                    success(point);
                    $('#loading-img').hide();
                }

            }
        }
    });
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


//show the map, with a pin in the given coords, and centered
function showMapWithPin(container,point,done) {
    
    var position=new AMap.LngLat(point.coords.longitude,point.coords.latitude);
        var mapObj=new AMap.Map(container,{
            view: new AMap.View2D({//创建地图二维视口
                center:position,//创建中心点坐标
                zoom:14, //设置地图缩放级别
                rotation:0 //设置地图旋转角度
            }),
            lang:"zh_cn"//设置地图语言类型，默认：中文简体
        });//创建地图实例
        //put a pin on the thouse
        var marker = new AMap.Marker({ //创建自定义点标注
            map:mapObj,
            position: new AMap.LngLat(point.coords.longitude, point.coords.latitude),
            offset: new AMap.Pixel(-32,-63),
            icon: "/\Content/\Images/\components/\map_pin-64.png"
        });
    done(mapObj,marker);
}
