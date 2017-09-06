package com.citygee.zhengwei.citygee;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.webkit.ConsoleMessage;
import android.webkit.GeolocationPermissions;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;
import com.citygee.zhengwei.citygee.Utility.UploadJob;
import com.citygee.zhengwei.citygee.Utility.UploadResult;
import com.citygee.zhengwei.citygee.Utility.UploadWorker;
import com.citygee.zhengwei.citygee.webclients.CityGeeWebClient;
import com.citygee.zhengwei.citygee.webclients.ObservableWebView;
import com.citygee.zhengwei.citygee.webclients.WebViewScrollHandler;
import com.google.gson.Gson;

import java.io.InputStream;


public class NormalActivity extends BaseActivity implements UploadWorker.OnUploadFinishListener {
    @Override
    public void onUploadFinish(String path) {
        if(path.equals("fail"))
        {
            Toast.makeText(this, "Upload Failed",
                    Toast.LENGTH_SHORT).show();
            return;
        }
        //decode the string find the uploaded url
        Gson gson=new Gson();
        UploadResult uploadedUrl = gson.fromJson(path, UploadResult.class);
        //find web view call the js
        mWebView.loadUrl("javascript:AndroidAppUpdateThumb('"+uploadedUrl.UploadedFileUrls[0]+"')");
        Toast.makeText(this, "上传成功",
                Toast.LENGTH_SHORT).show();
    }

    private static final int PICK_HOUSE_IMAGE_REQUEST = 1;
    private WebView mWebView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_normal);
        mWebViewClient=new CityGeeWebClient();
        //set up web view
        mWebView = (WebView)this.findViewById(R.id.web_view);
        WebSettings webSettings = mWebView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        //enable geo location
        webSettings.setGeolocationEnabled(true);
        webSettings.setGeolocationDatabasePath(getFilesDir().getPath());
        //set custom agent
        webSettings.setUserAgentString(getString(R.string.user_agent_string));

        mWebView.setWebChromeClient(new WebChromeClient() {
            //start of the anonymous web chrome class
            public void onGeolocationPermissionsShowPrompt(String origin, GeolocationPermissions.Callback callback) {
                callback.invoke(origin, true, false);
            }

            @Override
            public void onReceivedTitle(WebView view, String title) {
                super.onReceivedTitle(view, title);
                ((Activity)view.getContext()).setTitle(BasicUtility.generateTitle(title));
            }


            @Override
            public boolean onConsoleMessage(ConsoleMessage consoleMessage) {
                if(consoleMessage.message().equals(getString(R.string.intent_upload_trigger)))
                {
                    Log.v("ChromeClient", "upload photo triggered");
                    //todo start a dialog let user to take photo or select a photo
                    //start a intent to select photo
                    Intent intent = new Intent();
                    // Show only images, no videos or anything else
                    intent.setType("image/*");
                    intent.setAction(Intent.ACTION_GET_CONTENT);
                    // Always show the chooser (if there are multiple options available)
                    startActivityForResult(Intent.createChooser(intent, "请选择一张照片"), PICK_HOUSE_IMAGE_REQUEST);
                    return true;
                }
                if(consoleMessage.message().equals(getString(R.string.intent_login_trigger)))
                {
                    Log.v("ChromeClient", "require log in triggered");
                    BasicUtility.gotoLoginActivity(NormalActivity.this);
                    return true;
                }
                Log.v("ChromeClient", "not my message: "+consoleMessage.message());
                return true;
            }
        });
        ////end of the anonymous chrome client class and setter

        mWebView.setWebViewClient(mWebViewClient);
        mWebView.loadUrl(mUrlToLoad);
        addPageSpecificWeggit();
        ((ObservableWebView)mWebView).setOnScrollChangedHandler(new WebViewScrollHandler(this));
    }


    @Override
    protected void onActivityResult(int requestCode, int resultCode,
                                    Intent intent) {
        //user have chosen a image for a house
        if(requestCode==PICK_HOUSE_IMAGE_REQUEST)
        {

            try {
                // When an Image is picked
                if (requestCode == PICK_HOUSE_IMAGE_REQUEST && resultCode == RESULT_OK
                        && null != intent) {
                    // Get the Image from data
                    Uri selectedImage = intent.getData();
                    //get the file stream
                    InputStream inputStream = getContentResolver().openInputStream(selectedImage);
                    //show the progress bar
                    //start worker
                    UploadJob job = new UploadJob();
                    job.StreamToUpload=inputStream;
                    job.Url = mUploadImgUrl;
                    UploadWorker worker = new UploadWorker();
                    worker.setOnUploadFinishListener(this);
                    worker.execute(job);

                    Toast.makeText(this, "Upload Start",
                            Toast.LENGTH_SHORT).show();
                } else {
                    Toast.makeText(this, "You haven't picked Image",
                            Toast.LENGTH_LONG).show();
                }
            } catch (Exception e) {
                Toast.makeText(this, "Something went wrong", Toast.LENGTH_LONG)
                        .show();
            }

        }

        //get back from login activity
        //if user logged in refresh
        if(requestCode== BasicUtility.START_LOGIN  )
        {
            if(resultCode == RESULT_OK){
                //REFRESH
                mWebView.reload();
            }else{
                finish();
            }
        }
    }



}
