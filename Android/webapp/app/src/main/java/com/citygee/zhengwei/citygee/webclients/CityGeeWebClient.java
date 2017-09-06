package com.citygee.zhengwei.citygee.webclients;

import android.webkit.WebView;
import android.webkit.WebViewClient;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;

import java.net.MalformedURLException;
import java.net.URL;

/**
 * Created by zhengwei on 2015/7/1.
 */
public class CityGeeWebClient extends WebViewClient {
    @Override
    public boolean shouldOverrideUrlLoading(WebView view, String url) {
        URL parsedUrl;
        try {
            parsedUrl = new URL(url);
        } catch (MalformedURLException e) {
            e.printStackTrace();
            return false;
        }
        if(      (parsedUrl.getPath().toLowerCase().equals("/")|| parsedUrl.getPath().toLowerCase().equals("/feed") ||  parsedUrl.getPath().toLowerCase().equals("/house/near") )&&
                                parsedUrl.getQuery()!="search"
                ){
            BasicUtility.gotoMainActivity(view.getContext());
            return true;
        }
        if(parsedUrl.getPath().toLowerCase().equals("/account/login")){
            BasicUtility.gotoLoginActivity(view.getContext());
            return true;
        }
        if(parsedUrl.getPath().toLowerCase().contains("/myliked/checkins")||
                parsedUrl.getPath().toLowerCase().contains("/myliked/houses")){
            BasicUtility.gotoLikeActivity(view.getContext(),url);
            return true;
        }

        BasicUtility.gotoNormalActivity(view.getContext(), url);


        return true;
    }


}
