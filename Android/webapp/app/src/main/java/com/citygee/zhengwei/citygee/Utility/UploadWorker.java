package com.citygee.zhengwei.citygee.Utility;

import android.os.AsyncTask;
import android.text.TextUtils;
import android.util.Log;
import android.webkit.CookieManager;

import com.citygee.zhengwei.citygee.MyApp;
import com.citygee.zhengwei.citygee.R;

import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;

/**
 * Created by zhengwei on 2015/7/9.
 */
public class UploadWorker extends AsyncTask<UploadJob,Void,String> {

    private OnUploadFinishListener mListener;

    public interface OnUploadFinishListener{
        public void  onUploadFinish(String path);
    }
    @Override
    protected String doInBackground(UploadJob... params) {

        try {
            return uploadNow(params[0].StreamToUpload,params[0].Url);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }
        return "fail";
    }

    @Override
    protected void onPostExecute(String path) {
        if(mListener!=null)
        {
            mListener.onUploadFinish(path);
        }
    }

    public void setOnUploadFinishListener(OnUploadFinishListener listener){
        mListener=listener;
    }

    private String uploadNow(InputStream fileInputStream,String Url) throws MalformedURLException {

        URL connectURL=new URL(Url);
        String responseString="fail";
        String Title="file";
        String Description="file";
        String lineEnd = "\r\n";
        String twoHyphens = "--";
        String boundary = "*****";
        String Tag="fSnd";
        try
        {
            Log.e(Tag, "Starting Http File Sending to URL");

            // Open a HTTP connection to the URL
            HttpURLConnection conn = (HttpURLConnection)connectURL.openConnection();

            // Allow Inputs
            conn.setDoInput(true);

            // Allow Outputs
            conn.setDoOutput(true);

            // Don't use a cached copy.
            conn.setUseCaches(false);

            // Use a post method.
            conn.setRequestMethod("POST");

            conn.setRequestProperty("Connection", "Keep-Alive");

            conn.setRequestProperty("Content-Type", "multipart/form-data;boundary="+boundary);

            //set cookie
            CookieManager cookieManager = CookieManager.getInstance();
            if(cookieManager.getCookie(MyApp.jusGetResources().getString(R.string.url_web_root))!=null)
            {
                conn.setRequestProperty("Cookie",
                        cookieManager.getCookie(MyApp.jusGetResources().getString(R.string.url_web_root)));
            }

            DataOutputStream dos = new DataOutputStream(conn.getOutputStream());

            dos.writeBytes(twoHyphens + boundary + lineEnd);
            dos.writeBytes("Content-Disposition: form-data; name=\"title\""+ lineEnd);
            dos.writeBytes(lineEnd);
            dos.writeBytes(Title);
            dos.writeBytes(lineEnd);
            dos.writeBytes(twoHyphens + boundary + lineEnd);

            dos.writeBytes("Content-Disposition: form-data; name=\"description\""+ lineEnd);
            dos.writeBytes(lineEnd);
            dos.writeBytes(Description);
            dos.writeBytes(lineEnd);
            dos.writeBytes(twoHyphens + boundary + lineEnd);

            dos.writeBytes("Content-Disposition: form-data; name=\"uploadedfile\";filename=\"image.jpg\"" + lineEnd);
            dos.writeBytes(lineEnd);

            Log.e(Tag,"Headers are written");

            // create a buffer of maximum size
            int bytesAvailable = fileInputStream.available();

            int maxBufferSize = 1024;
            int bufferSize = Math.min(bytesAvailable, maxBufferSize);
            byte[ ] buffer = new byte[bufferSize];

            // read file and write it into form...
            int bytesRead = fileInputStream.read(buffer, 0, bufferSize);

            while (bytesRead > 0)
            {
                dos.write(buffer, 0, bufferSize);
                bytesAvailable = fileInputStream.available();
                bufferSize = Math.min(bytesAvailable,maxBufferSize);
                bytesRead = fileInputStream.read(buffer, 0,bufferSize);
            }
            dos.writeBytes(lineEnd);
            dos.writeBytes(twoHyphens + boundary + twoHyphens + lineEnd);

            // close streams
            fileInputStream.close();

            dos.flush();

            Log.v(Tag, "File Sent, Response Code: " + String.valueOf(conn.getResponseCode()));

            InputStream is = conn.getInputStream();

            // retrieve the response from server
            int ch;

            StringBuffer b =new StringBuffer();
            while( ( ch = is.read() ) != -1 ){ b.append( (char)ch ); }
            String s=b.toString();
            Log.v(Tag,"Response: "+s);
            responseString=s;
            dos.close();
        }
        catch (MalformedURLException ex)
        {
            Log.e(Tag, "URL error: " + ex.getMessage(), ex);
        }

        catch (IOException ioe)
        {
            Log.e(Tag, "IO error: " + ioe.getMessage(), ioe);
        }

        return responseString;
    }


}
