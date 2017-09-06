package com.citygee.zhengwei.citygee.webclients;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.webkit.GeolocationPermissions;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;
import android.webkit.WebView;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;

/**
 * Created by zhengwei on 2015/7/3.
 */
public class AllowGeoWebChromeClient extends WebChromeClient {
    @Override
    public void onGeolocationPermissionsShowPrompt(String origin, GeolocationPermissions.Callback callback) {
        callback.invoke(origin, true, false);
    }


}
