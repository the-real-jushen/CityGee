package com.citygee.zhengwei.citygee;

import android.app.Application;
import android.content.Context;
import android.content.res.Resources;

/**
 * Created by zhengwei on 2015/7/2.
 */
public class MyApp extends Application {

    public static long apkDownloadId;
    private static Context mContext;

    public static Resources jusGetResources() {
        return mContext.getResources();
    }
    public static Context jusGetContext() {
        return mContext;
    }

    public void onCreate() {
        super.onCreate();
        mContext = getApplicationContext();
    }
}