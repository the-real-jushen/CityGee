package com.citygee.zhengwei.citygee;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebSettings;
import android.webkit.WebView;


import com.citygee.zhengwei.citygee.webclients.AllowGeoWebChromeClient;
import com.citygee.zhengwei.citygee.webclients.CityGeeWebClient;
import com.citygee.zhengwei.citygee.webclients.ObservableWebView;
import com.citygee.zhengwei.citygee.webclients.WebViewScrollHandler;
import com.github.clans.fab.FloatingActionButton;


public class MainFragment extends Fragment {

    private String mUrlToLoad;
    public static final String URL_TO_LOAD="mUrlToLoad";
    public MainFragment() {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            mUrlToLoad=getArguments().getString(URL_TO_LOAD);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView = inflater.inflate(R.layout.fragment_main, container, false);

        //set up web view
        WebView webView = (WebView)rootView.findViewById(R.id.web_view);
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        //enable geo location
        webSettings.setGeolocationEnabled(true);
        webSettings.setGeolocationDatabasePath(getActivity().getFilesDir().getPath());
        //set custom agent
        webSettings.setUserAgentString(getString(R.string.user_agent_string));

        webView.setWebViewClient(new CityGeeWebClient());
        webView.setWebChromeClient(new AllowGeoWebChromeClient());
        webView.loadUrl(mUrlToLoad);
        ((ObservableWebView)webView).setOnScrollChangedHandler(new WebViewScrollHandler(getActivity()));
        return rootView;
    }

}


