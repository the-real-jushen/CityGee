package com.citygee.zhengwei.citygee.webclients;

import android.content.Context;
import android.util.Log;
import android.webkit.CookieManager;
import android.webkit.WebView;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.MyApp;
import com.citygee.zhengwei.citygee.R;

import java.net.MalformedURLException;
import java.net.URL;

/**
 * Created by zhengwei on 2015/7/3.
 */
public class RegisterWebClient extends CityGeeWebClient{
    private final  String LOG_TAG=this.getClass().getName();
    private OnRegisterFinishedListener mMyListener;
    private String mReturnUrl;

    public RegisterWebClient(){
        super ();
        //force log out
        CookieManager cookieManager = CookieManager.getInstance();
        cookieManager.removeAllCookie();

    }

    @Override
    public boolean shouldOverrideUrlLoading(WebView view, String url) {
        //check if it is the register url
        URL requestUrl = null;
        try {
            requestUrl=new URL(url);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }
        if (requestUrl==null)
        {
            return false;
        }
        if(requestUrl.getPath()!=MyApp.jusGetResources().getString(R.string.path_web_register)|| requestUrl.getAuthority()!= MyApp.jusGetResources().getString(R.string.web_domain_name))
        {
            //you clicked wrong link
            //todo may be useing a dialog
            Toast.makeText(MyApp.jusGetContext(), "只能点击“注册”!!", Toast.LENGTH_SHORT).show();
            return true;
        }
        return true;
    }

    public void setOnRegisterFinishedListener(OnRegisterFinishedListener listener){
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
                mMyListener.OnRegisterFinished(true);
                return;
            }
            mMyListener.OnRegisterFinished(false);

        }
    }

    public interface OnRegisterFinishedListener{
        //todo add redirect here
        public void OnRegisterFinished(boolean isLoggedIn);
    }
}
