package com.citygee.zhengwei.citygee;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.view.View;

import com.citygee.zhengwei.citygee.Utility.MainFragmentPagerAdapterData;

/**
 * Created by zhengwei on 2015/7/2.
 */
public class MainFragmentPagerAdapter extends FragmentPagerAdapter {

    private MainFragment mNewsView;
    private MainFragment mNearView;
    private MainFragment[] mViews;
    private MainFragmentPagerAdapterData[] mData;


    @Override
    public CharSequence getPageTitle(int position) {
        return mData[position].title;
        /*
        if (position==0)
        {
            //return news page
            return MyApp.jusGetResources().getString(R.string.title_tab_news);
        }
        //return near page
        return MyApp.jusGetResources().getString(R.string.title_tab_near);
        */
    }

    public MainFragmentPagerAdapter(FragmentManager fm,MainFragmentPagerAdapterData[] data){
        super(fm);
        mData=data;
        mViews=new MainFragment[data.length];
        for(int i=0;i<data.length;i++){
            mViews[i]=new MainFragment();
            Bundle args = new Bundle();
            args.putString(MainFragment.URL_TO_LOAD, data[i].url);
            mViews[i].setArguments(args);
        }
        /*
        mNewsView=new MainFragment();
        mNearView=new MainFragment();
        Bundle args = new Bundle();
        args.putString(MainFragment.URL_TO_LOAD, MyApp.jusGetResources().getString(R.string.url_web_news));
        mNewsView.setArguments(args);
        args = new Bundle();
        args.putString(MainFragment.URL_TO_LOAD,MyApp.jusGetResources().getString(R.string.url_web_near));
        mNearView.setArguments(args);*/

    }

    @Override
    public Fragment getItem(int position) {
        /*if (position==0)
        {
            //return news page
            return mNewsView;
        }
        //return near page
        return mNearView;*/
        return mViews[position];

    }

    @Override
    public int getCount() {
        return mData.length;
    }
}
