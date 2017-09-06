package com.citygee.zhengwei.citygee;

import android.content.SharedPreferences;
import android.os.Handler;
import android.support.v4.view.ViewPager;
import android.os.Bundle;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.Utility.MainFragmentPagerAdapterData;
import com.citygee.zhengwei.citygee.Utility.UpdateHelper;
import com.citygee.zhengwei.citygee.Utility.UpgradeDialogFragment;
import com.citygee.zhengwei.citygee.Utility.UpgradeInfo;


public class MainActivity extends TabViewaActivity implements UpdateHelper.OnGetNeedUpgradeAsyncListener {

    MainFragmentPagerAdapter mAdapter;

    ViewPager mPager;
    private UpdateHelper checker;
    private Boolean exit = false;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);
        //check upgrade
        SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
        boolean needUpgrade = settings.getBoolean(getString(R.string.pref_key_need_upgrade), false);
        if (needUpgrade){

            //reset
            SharedPreferences.Editor editor = settings.edit();
            editor.putBoolean(getString(R.string.pref_key_need_upgrade),false);
            editor.commit();
        }else  {
            //todo add check the first time in a day
            checker=new UpdateHelper();
            checker.getNeedUpgradeAsync(this);
        }

         addPageSpecificWeggit();
        setTitle("城迹");
    }


    @Override
    public MainFragmentPagerAdapterData[] getViewTabData() {
        MainFragmentPagerAdapterData[] data=new MainFragmentPagerAdapterData[2];
        data[0]=new MainFragmentPagerAdapterData();
        data[1]=new MainFragmentPagerAdapterData();
        data[0].title=getString(R.string.title_tab_near);
        data[0].url=getString(R.string.url_web_near);
        data[1].title=getString(R.string.title_tab_news);
        data[1].url=getString(R.string.url_web_news);
        return data;
    }

    @Override
    public void onBackPressed() {
        if (exit) {
            finish(); // finish activity
        } else {
            Toast.makeText(this, "再按一下退出",
                    Toast.LENGTH_SHORT).show();
            exit = true;
            new Handler().postDelayed(new Runnable() {
                @Override
                public void run() {
                    exit = false;
                }
            }, 3 * 1000);

        }

    }


    @Override
    public void OnGetNeedUpgradeAsync(UpgradeInfo info) {
        //if need upgrade show a dialog here
        if (info==null){
            return;
        }
        if(info.needUpGrade) {
            try{
                UpgradeDialogFragment upDialog = new UpgradeDialogFragment();
                Bundle arg=new Bundle();
                arg.putString("versionName",info.versionName);
                arg.putString("releaseNote",info.releaseNote);
                upDialog.setArguments(arg);
                upDialog.show(getSupportFragmentManager(),"Upgrade");
            }
            catch (Exception e){
                //next time open the main activity will prompt upgrade without polling the web api
                SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
                SharedPreferences.Editor editor = settings.edit();
                editor.putBoolean(getString(R.string.pref_key_need_upgrade),true);
                editor.commit();
            }

        }
    }



}
