package com.citygee.zhengwei.citygee.Utility;

import android.app.AlertDialog;
import android.app.Dialog;
import android.app.DownloadManager;
import android.content.Context;
import android.content.DialogInterface;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.support.v4.app.DialogFragment;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.MyApp;
import com.citygee.zhengwei.citygee.R;

/**
 * Created by zhengwei on 2015/7/11.
 */


public class UpgradeDialogFragment extends DialogFragment {


    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        // Use the Builder class for convenient dialog construction
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        Bundle data=getArguments();
        builder.setTitle("升级了")
                .setMessage(data.getString("releaseNote") + " 版本号：" + data.get("versionName"))
                .setPositiveButton("升级", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        // todo start the download manager here
                        String url = getString(R.string.url_app_download);
                        DownloadManager.Request request = new DownloadManager.Request(Uri.parse(url));
                        request.setDescription("CityGee upgrade");
                        request.setTitle("城迹更新下载");
                        if (isExternalStorageWritable() == false) {
                            Toast.makeText(getActivity(), "无法下载，没有找到存储", Toast.LENGTH_SHORT).show();
                            return;
                        }
                        request.setDestinationInExternalFilesDir(getActivity(),null, "citygee.apk");

                        // get download service and enqueue file
                        DownloadManager manager = (DownloadManager) getActivity().getSystemService(Context.DOWNLOAD_SERVICE);
                        MyApp.apkDownloadId= manager.enqueue(request);
                        Toast.makeText(getActivity(), "下载开始", Toast.LENGTH_SHORT).show();
                    }
                })
                .setNegativeButton("下次再说", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        // User cancelled the dialog
                    }
                });
        // Create the AlertDialog object and return it
        return builder.create();
    }


    /* Checks if external storage is available for read and write */
    public boolean isExternalStorageWritable() {
        String state = Environment.getExternalStorageState();
        if (Environment.MEDIA_MOUNTED.equals(state)) {
            return true;
        }
        return false;
    }

}