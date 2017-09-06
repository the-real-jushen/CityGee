package com.citygee.zhengwei.citygee;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.support.v4.view.MenuItemCompat;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.support.v7.widget.ShareActionProvider;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.webkit.WebView;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;
import com.citygee.zhengwei.citygee.webclients.CityGeeWebClient;
import com.github.clans.fab.FloatingActionButton;

import java.net.MalformedURLException;
import java.net.URL;


public class BaseActivity extends ActionBarActivity {

    protected CityGeeWebClient mWebViewClient;
    protected String mUrlToLoad;
    //used only in normal activity but it is easier to set it here
    protected   String mUploadImgUrl;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        getWindow().requestFeature(Window.FEATURE_ACTION_BAR_OVERLAY);
        super.onCreate(savedInstanceState);
        getIntentUrl();
        //setContentView(R.layout.activity_base);
        //addPageSpecificWeggit();
    }




    protected void addPageSpecificWeggit(){
        //use the url to determine what menu and buttons should be added
        URL parsedUrl = null;
        try {
            parsedUrl = new URL(mUrlToLoad);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }

        View fab;
        LayoutInflater vi = (LayoutInflater) getApplicationContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        if (parsedUrl.getPath().toLowerCase().contains("/house/detail/")){

            fab = vi.inflate(R.layout.fab_menu_house, null);


        }else if (parsedUrl.getPath().toLowerCase().contains("/checkin/detail/")) {
            fab = vi.inflate(R.layout.fab_menu_checkin, null);
        }else{
            fab = vi.inflate(R.layout.fab_menu_normal, null);
        }

        // insert into main view
        final ViewGroup insertPoint = (ViewGroup) findViewById(R.id.root_view);
        insertPoint.addView(fab);
        //done hook up event handler here
        //discover, can be always found
        ((FloatingActionButton)findViewById(R.id.fab_item_discover)).setOnClickListener(new View.OnClickListener(){

            @Override
            public void onClick(View v) {
                BasicUtility.gotoNormalActivity(insertPoint.getContext(),getString(R.string.url_web_discover));
            }
        });
        //below the fab requires an id
        String path=parsedUrl.getPath();
        if(path.toLowerCase().contains("detail/")==false){
            //not even a trace of id
            return;
        }
        String t_id=path.toLowerCase().substring(path.lastIndexOf("detail/")+7);

        if(t_id==null || t_id.isEmpty()){
            //no id specific button should be appearing
            return;
        }
        if(t_id.contains("/")){
            t_id=t_id.substring(0,t_id.indexOf("/"));
        }
        final String id=t_id;
        //like, should always appearing if there is a valid id
        FloatingActionButton fab_item = (FloatingActionButton) findViewById(R.id.fab_item_like);
        if(fab_item!=null){
            fab_item.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    BasicUtility.toggleLike(insertPoint.getContext(),id);
                }
            });
        }
        //checkin
        fab_item = (FloatingActionButton) findViewById(R.id.fab_item_checkin);
        if(fab_item!=null){
            fab_item.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    BasicUtility.gotoNormalActivity(insertPoint.getContext(),v.getContext().getString(R.string.url_web_new_ckeckin)+id);
                }
            });
        }

    }




    protected String getIntentUrl(){
        Intent intent=getIntent();
        mUrlToLoad=intent.getStringExtra(getString(R.string.intent_start_url));
        mUploadImgUrl=getString(R.string.url_web_upload_house_image);
        return mUrlToLoad;
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        if(BasicUtility.getLoggedInPreference(this)){
            getMenuInflater().inflate(R.menu.menu_loggedin, menu);
        }
        else{
            getMenuInflater().inflate(R.menu.menu_guest, menu);
        }
        getMenuInflater().inflate(R.menu.menu_common, menu);
        getMenuInflater().inflate(R.menu.menu_search, menu);
        return true;
    }



    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        menu.clear();
        // Inflate the menu; this adds items to the action bar if it is present.
        if(BasicUtility.getLoggedInPreference(this)){
            getMenuInflater().inflate(R.menu.menu_loggedin, menu);
        }
        else{
            getMenuInflater().inflate(R.menu.menu_guest, menu);
        }
        getMenuInflater().inflate(R.menu.menu_common, menu);
        getMenuInflater().inflate(R.menu.menu_search, menu);

        //add share icon if it is a house detail or checkin detail
        URL parsedUrl = null;
        try {
            parsedUrl = new URL(mUrlToLoad);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }

        if (parsedUrl.getPath().toLowerCase().contains("/house/detail/")||parsedUrl.getPath().toLowerCase().contains("/checkin/detail/")) {
            getMenuInflater().inflate(R.menu.menu_share, menu);
            MenuItem item = menu.findItem(R.id.action_share);

            // Fetch and store ShareActionProvider
            ShareActionProvider shareActionProvider = (ShareActionProvider) MenuItemCompat.getActionProvider(item);
            //whoever provide the content should provide the share icon
            //prepare the share button when preparing the menu
            Intent shareIntent = new Intent();
            shareIntent.setAction(Intent.ACTION_SEND);
            shareIntent.putExtra(Intent.EXTRA_TEXT, mUrlToLoad);
            shareIntent.setType("text/plain");
            shareIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_WHEN_TASK_RESET);
            if (shareActionProvider != null) {
                shareActionProvider.setShareIntent(shareIntent);
            }
        }

        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        //todo handle menu click here
        switch (id){
            case R.id.action_login:{
                BasicUtility.gotoLoginActivity(this);
                break;
            }
            case R.id.action_logout:{
                BasicUtility.gotoLoginActivity(this);
                break;
            }
            case R.id.action_main:{
                BasicUtility.gotoMainActivity(this);
                break;
            }
            case R.id.action_profile:{
                //actually id is not needed here, this is for test get uid function
                //todo you dont need id here
                SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
                BasicUtility.gotoNormalActivity(this,getString(R.string.url_web_profile)+"/"+settings.getString(getString(R.string.pref_key_uid), "xx"));
                break;
            }
            case R.id.action_feedback:{
                BasicUtility.gotoFeedbackActivity(this);
                break;
            }
            case R.id.action_search:{
                BasicUtility.gotoSearchActivity(this);
                break;
            }
            default:
                break;

        }


        return true;
    }




}
