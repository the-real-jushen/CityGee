package com.citygee.zhengwei.citygee;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v4.view.ViewPager;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;
import com.citygee.zhengwei.citygee.Utility.MainFragmentPagerAdapterData;

import java.net.MalformedURLException;
import java.net.URL;

/**
 * Created by zhengwei on 2015/7/13.
 */
public class LikeActivity extends TabViewaActivity {

    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);
        URL url = null;
        try {
            url=new URL(mUrlToLoad);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }
        String path = url.getPath();
        String id = path.substring(path.lastIndexOf("/")+1);
        SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
        String myid = settings.getString(getString(R.string.pref_key_uid), "xx");
        if(id.equals(myid)){
            setTitle("我的点赞");
        }else {
            setTitle("Ta的点赞");
        }

    }

    @Override
    public MainFragmentPagerAdapterData[] getViewTabData() {
        URL url = null;
        try {
            url=new URL(mUrlToLoad);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }
        String path = url.getPath();
        String id = path.substring(path.lastIndexOf("/")+1);
        MainFragmentPagerAdapterData[] data=new MainFragmentPagerAdapterData[2];
        data[0]=new MainFragmentPagerAdapterData();
        data[1]=new MainFragmentPagerAdapterData();
        data[0].title=getString(R.string.title_tab_house);
        data[0].url=getString(R.string.url_web_liked_house)+id;
        data[1].title=getString(R.string.title_tab_checkin);
        data[1].url=getString(R.string.url_web_liked_checkin)+id;
        return data;
    }
}
