package com.citygee.zhengwei.citygee;

import android.content.Intent;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.widget.Toast;


import com.citygee.zhengwei.citygee.Utility.BasicUtility;
import com.citygee.zhengwei.citygee.webclients.RegisterWebClient;


public class RegisterActivity extends ActionBarActivity implements RegisterWebClient.OnRegisterFinishedListener {
    @Override
    public void OnRegisterFinished(boolean isLoggedIn) {
        if (isLoggedIn) {
            Toast.makeText(this, "注册成功", Toast.LENGTH_SHORT).show();
            BasicUtility.setLogIn(this,true);
            finish();
        } else {
            Toast.makeText(this,"好像失败了", Toast.LENGTH_SHORT).show();
        }
    }



    private RegisterWebClient mWebViewClient;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);

        //set up web view
        WebView webView = (WebView)findViewById(R.id.web_view);
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        //set custom agent
        webSettings.setUserAgentString(getString(R.string.user_agent_string));
        //todo set web view client
        mWebViewClient=new RegisterWebClient();
        mWebViewClient.setOnRegisterFinishedListener(this);
        webView.setWebViewClient(mWebViewClient);
        webView.loadUrl(getString(R.string.url_web_register));


    }



}
