package com.citygee.zhengwei.citygee.webclients;

import android.content.Context;
import android.util.Log;
import android.webkit.CookieManager;
import android.webkit.WebView;

import com.citygee.zhengwei.citygee.R;

/**
 * Created by zhengwei on 2015/7/2.
 */
public class LoginWebClient extends CityGeeWebClient{
    private final  String LOG_TAG=this.getClass().getName();
    private OnLogInFinishedListener mMyListener;

    public LoginWebClient(){
        super ();


    }

    @Override
    public boolean shouldOverrideUrlLoading(WebView view, String url) {
        return true;
    }

    public void setOnLogInFinishedListener(OnLogInFinishedListener listener){
        mMyListener=listener;
    }


    public void onPageFinished(WebView view, String url) {
        super.onPageFinished(view, url);
        //get the cookie manager and check if we have the login cookie
        Context activity=view.getContext();
        CookieManager cookieManager = CookieManager.getInstance();
        String cookie=cookieManager.getCookie(activity.getString(R.string.url_web_root));
        if (cookie!=null ) {
            Log.v(LOG_TAG, cookie);
            Log.v(LOG_TAG, url);
        }
        if(mMyListener!=null){
            if(cookie!=null && cookie.contains(activity.getString(R.string.cookie_name_login)))
            {
                mMyListener.OnLogInFinished(true);
                return;
            }
            mMyListener.OnLogInFinished(false);

        }
    }

    public interface OnLogInFinishedListener{
        public void OnLogInFinished(boolean isLoggedIn);
    }
}
