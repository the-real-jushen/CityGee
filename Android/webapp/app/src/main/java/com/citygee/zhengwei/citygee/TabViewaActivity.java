package com.citygee.zhengwei.citygee;

import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.NonNull;
import android.support.v4.view.ViewPager;
import android.util.AttributeSet;
import android.view.View;
import android.view.Window;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.Utility.MainFragmentPagerAdapterData;
import com.citygee.zhengwei.citygee.Utility.UpdateHelper;
import com.citygee.zhengwei.citygee.Utility.UpgradeDialogFragment;
import com.citygee.zhengwei.citygee.Utility.UpgradeInfo;

/**
 * Created by zhengwei on 2015/7/13.
 * just like other activities in this app but can display pages in tabs(viewpager)
 */
public class TabViewaActivity extends BaseActivity{
    MainFragmentPagerAdapter mAdapter;

    ViewPager mPager;



    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);
        getIntentUrl();
        //set the two tab fragment adapter

        mAdapter = new MainFragmentPagerAdapter(getSupportFragmentManager(),getViewTabData());

        mPager = (ViewPager)findViewById(R.id.pager);
        mPager.setAdapter(mAdapter);
        //webview is set up in fragment
        //we hae viewpager so no elevation
        getSupportActionBar().setElevation(0);
    }



    //override this if you want to have different tabs
    public  MainFragmentPagerAdapterData[] getViewTabData(){

        MainFragmentPagerAdapterData[] data=new MainFragmentPagerAdapterData[2];
        data[0]=new MainFragmentPagerAdapterData();
        data[1]=new MainFragmentPagerAdapterData();
        data[0].title=getString(R.string.title_tab_house);
        data[0].url=getString(R.string.url_web_near);
        data[1].title=getString(R.string.title_tab_checkin);
        data[1].url=getString(R.string.url_web_news);
        return data;
    }



}
