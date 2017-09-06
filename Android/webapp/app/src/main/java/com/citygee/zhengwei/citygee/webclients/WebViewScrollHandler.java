package com.citygee.zhengwei.citygee.webclients;

import android.app.Activity;
import android.support.v7.app.ActionBarActivity;
import android.util.Log;
import android.view.View;

import com.citygee.zhengwei.citygee.R;
import com.github.clans.fab.FloatingActionButton;
import com.github.clans.fab.FloatingActionMenu;

/**
 * Created by zhengwei on 2015/7/14.
 * show and hide action bar and the fab when scrolling
 */
public class WebViewScrollHandler implements ObservableWebView.OnScrollChangedHandler{
    private final Activity mActivity;

    public WebViewScrollHandler(Activity activity){
        mActivity =activity;
    }

    @Override
    public void onScroll(int l, int t, int distanceY) {
        //Do stuff
        Log.d("WebViewScroll", "We Scrolled etc..." + distanceY);

        if (distanceY > 100) {
            hideFab(true);
            hideActionBar(true);
        }
        if (distanceY < -50) {
            hideFab(false);
            hideActionBar(false);
        }
    }

    private void hideActionBar(boolean hideOrShow){
        if(hideOrShow==true){
            //hide
            ((ActionBarActivity)mActivity).getSupportActionBar().hide();
        }else{
            ((ActionBarActivity)mActivity).getSupportActionBar().show();
        }
    }


    //since the fucking fab and fab menu is not the same thing i have to show hide them saperately
    private void hideFab(boolean hideOrShow){
        View fab=mActivity.findViewById(R.id.fab_menu_root);

        //if not normal fab
        if (fab!=null){
            //the normal fab
            if(hideOrShow==true){
                //hide
                ((FloatingActionMenu)fab).hideMenuButton(true);
            }else{
                ((FloatingActionMenu) fab).showMenuButton(true);
            }

        }else{
            //fab menu
            fab=mActivity.findViewById(R.id.fab_item_discover);
            if(hideOrShow==true){
                //hide
                ((FloatingActionButton)fab).hide(true);
            }else{
                ((FloatingActionButton) fab).show(true);
            }
        }


    }
}
