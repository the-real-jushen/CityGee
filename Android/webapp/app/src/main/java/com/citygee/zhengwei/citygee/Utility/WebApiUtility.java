package com.citygee.zhengwei.citygee.Utility;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.webkit.CookieManager;

import com.citygee.zhengwei.citygee.MyApp;
import com.citygee.zhengwei.citygee.R;
import com.google.gson.Gson;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

/**
 * Created by zhengwei on 2015/7/12.
 */
public class WebApiUtility {

    private OnGetAppUserListener mListener;

    public interface OnGetAppUserListener{
        public void  onGetAppUser(AppUser user);
    }
    public void getUserIdAsync(Context context,OnGetAppUserListener listener){
        if (BasicUtility.getLoggedInPreference(context)==false){
            return ;
        }
        mListener=listener;
        new AsyncTask<Void,Void,AppUser>(){

            @Override
            protected AppUser doInBackground(Void... params) {
                String result=simpleGetRequestWithAppCookie(MyApp.jusGetResources().getString(R.string.url_app_appuser));
                AppUser appUser=new Gson().fromJson(result,AppUser.class);
                return appUser;
            }

            @Override
            protected void onPostExecute(AppUser s) {
                mListener.onGetAppUser(s);
            }
        }.execute();
    }

    public static String simpleGetRequestWithAppCookie(String urlStr){
        HttpURLConnection urlConnection = null;
        BufferedReader reader = null;

        // Will contain the raw JSON response as a string.
        String resultJsonStr = null;

        try {

            URL url = new URL(urlStr);

            // Create the request to OpenWeatherMap, and open the connection
            urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setRequestMethod("GET");
            //set cookie
            CookieManager cookieManager = CookieManager.getInstance();
            if(cookieManager.getCookie(MyApp.jusGetResources().getString(R.string.url_web_root))!=null)
            {
                urlConnection.setRequestProperty("Cookie",
                        cookieManager.getCookie(MyApp.jusGetResources().getString(R.string.url_web_root)));
            }
            urlConnection.connect();

            // Read the input stream into a String
            InputStream inputStream = urlConnection.getInputStream();
            StringBuffer buffer = new StringBuffer();
            if (inputStream == null) {
                // Nothing to do.
                return null;
            }
            reader = new BufferedReader(new InputStreamReader(inputStream));

            String line;
            while ((line = reader.readLine()) != null) {
                // Since it's JSON, adding a newline isn't necessary (it won't affect parsing)
                // But it does make debugging a *lot* easier if you print out the completed
                // buffer for debugging.
                buffer.append(line + "\n");
            }

            if (buffer.length() == 0) {
                // Stream was empty.  No point in parsing.
                return null;
            }
            resultJsonStr = buffer.toString();
        } catch (IOException e) {
            Log.e("CheckVersion", "Error ", e);
            // If the code didn't successfully get the weather data, there's no point in attemping
            // to parse it.
            return null;
        } finally{
            if (urlConnection != null) {
                urlConnection.disconnect();
            }
            if (reader != null) {
                try {
                    reader.close();
                } catch (final IOException e) {
                    Log.e("CheckVersion", "Error closing stream", e);
                }
            }
        }
        return resultJsonStr;
    }





}
