package com.citygee.zhengwei.citygee.restclient;

import android.content.SharedPreferences;

import com.citygee.zhengwei.citygee.MyApp;
import com.citygee.zhengwei.citygee.R;
import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.AsyncHttpResponseHandler;
import com.loopj.android.http.TextHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.apache.http.util.EncodingUtils;

/**
 * Created by zhengwei on 2015/7/16.
 */
public class RestClient {



    public static void postFeedbackAsync(String content,TextHttpResponseHandler handler){
        AsyncHttpClient client = new AsyncHttpClient();
        //todo add feedback url
        String url= MyApp.jusGetResources().getString(R.string.url_web_feedback);
        RequestParams params = new RequestParams();
        params.put("Content", content);
        SharedPreferences settings = MyApp.jusGetContext().getSharedPreferences( MyApp.jusGetContext().getString(R.string.pref_name_default), 0);
        params.put("CreatedUrl", "Android App");
        params.put("UserName", settings.getString(MyApp.jusGetContext().getString(R.string.pref_key_username), "Anonymous"));
        client.post(url,params,handler);
    }
}
