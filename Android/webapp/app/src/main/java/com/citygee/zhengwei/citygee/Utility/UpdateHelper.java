package com.citygee.zhengwei.citygee.Utility;

import android.content.Context;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.AsyncTask;
import android.util.Log;
import android.view.View;

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
 * Created by zhengwei on 2015/7/11.
 */
public class UpdateHelper {
    OnGetNeedUpgradeAsyncListener mListener;
    public interface OnGetNeedUpgradeAsyncListener{
        public void OnGetNeedUpgradeAsync(UpgradeInfo info);
    }


    public void getNeedUpgradeAsync(OnGetNeedUpgradeAsyncListener listener){
        mListener=listener;
        new checkUpGradeTask().execute();
    }

    private class checkUpGradeTask extends AsyncTask<Void, Void,UpgradeInfo>{

        @Override
        protected UpgradeInfo doInBackground(Void... params) {
            HttpURLConnection urlConnection = null;
            BufferedReader reader = null;

            // Will contain the raw JSON response as a string.
            String resultJsonStr = null;

            try {

                URL url = new URL(MyApp.jusGetResources().getString(R.string.url_app_version));

                // Create the request to OpenWeatherMap, and open the connection
                urlConnection = (HttpURLConnection) url.openConnection();
                urlConnection.setRequestMethod("GET");
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
            UpgradeInfo info= new Gson().fromJson(resultJsonStr,UpgradeInfo.class);
            //get the version code
            int verNum=0;
            try {
                Context context = MyApp.jusGetContext();
                verNum=context.getPackageManager().getPackageInfo(context.getPackageName(), 0).versionCode;
            } catch (PackageManager.NameNotFoundException e) {
                e.printStackTrace();
            }
            //if lower then need to upgrade
            if(verNum<info.versionNum)
            {
                info.needUpGrade=true;
            }
            else{
                info.needUpGrade=false;
            }
            return info;
        }

        @Override
        protected void onPostExecute(UpgradeInfo info) {
            mListener.OnGetNeedUpgradeAsync(info);
        }
    }


}
